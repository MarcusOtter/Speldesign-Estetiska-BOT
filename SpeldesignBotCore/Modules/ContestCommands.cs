using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using SpeldesignBotCore.Contests;
using SpeldesignBotCore.Entities;
using System;
using Discord.WebSocket;

namespace SpeldesignBotCore.Modules
{
    public class ContestCommands : ModuleBase<SocketCommandContext>
    {
        private readonly ContestHandler _contestHandler;

        public ContestCommands(ContestHandler contestHandler)
        {
            _contestHandler = contestHandler;
        }

        [Command("contest create")]
        [Alias("contest new", "contest add", "contest start")]
        [Summary("Creates a new contest and starts it"), Remarks("contest create [contest title]")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        public async Task NewContestSetup([Remainder] string contestTitle)
        {
            if (contestTitle.Length > 100)
            {
                await ReplyAsync("The contest title cannot be over 100 characters long.");
                return;
            }

            if (_contestHandler.ContestTitleExists(contestTitle))
            {
                await ReplyAsync("That contest title already exists. Try adding the year and the month to the title to make it unique.");
                return;
            }

            var contest = new Contest(contestTitle, Context.Channel.Id);

            _contestHandler.SaveNewContest(contest);

            var embed = new EmbedBuilder()
                .WithTitle("🏆 New contest created! 🏆")
                .AddField("Contest title", $"**{contest.Title}**")
                .AddField("How to enter the contest",
                    $"Make a contest submission in this channel ({((ITextChannel) Context.Channel).Mention}) by simply sending __**one**__ message with any images/links/text of your submission. " +
                    "Sending multiple messages will not work, so make sure to add all attachments to the message before sending it.")
                .AddField("Other important information", 
                    ":warning: **Please __do not__ send questions or other messages in this channel!**\n" +
                    "This channel is only used for submissions.\n\n" +
                    $"*Make sure that {Context.Client.CurrentUser.Mention} is online when you submit!*")
                .WithColor(new Color(118, 196, 177))
                .WithFooter($"🎁 Enter the contest to have a chance to win rewards once it closes!");

            var contestInstructions = await ReplyAsync("", embed: embed.Build());
            await contestInstructions.PinAsync();

            await Context.Message.DeleteAsync();
        }

        [Command("contest submissions")]
        [Alias("contest entries")]
        [Summary("Lists all the submissions in the given contest"), Remarks("contest submissions [contest title]")]
        public async Task ListSubmissions([Remainder] string contestTitle)
        {
            if (!_contestHandler.ContestTitleExists(contestTitle))
            {
                await ReplyAsync($"A contest with the name \"{contestTitle}\" does not exist.");
                return;
            }

            var contest = _contestHandler.GetContestByTitle(contestTitle);

            if (contest.Submissions is null || contest.Submissions.Count == 0)
            {
                await ReplyAsync("There are no submissions for this contest yet.");
                return;
            }

            var contestChannel = Context.Guild.GetTextChannel(contest.SubmissionChannelId);
            var embedBuilder = new EmbedBuilder()
                .WithTitle($"Submissions for the contest \"{contest.Title}\"")
                .WithColor(new Color(118, 196, 177));

            foreach (var submission in contest.Submissions)
            {
                var author = Context.Guild.GetUser(submission.AuthorId);
                var message = await contestChannel.GetMessageAsync(submission.MessageId);

                if (author is null)
                {
                    embedBuilder.AddField("Missing user", "<missing message>");
                    continue;
                }

                if (message is null)
                {
                    embedBuilder.AddField($"{author.Nickname ?? author.Username}", "<missing message>");
                    continue;
                }

                embedBuilder.AddField(
                    $"{author.Nickname ?? author.Username} ({message.Timestamp.ToLocalTime().ToString("dd/MM HH:mm")})",
                    $"[See submission](https://discordapp.com/channels/{Context.Guild.Id}/{contestChannel.Id}/{message.Id})",
                    inline: true);
            }

            await ReplyAsync("", embed: embedBuilder.Build());
        }

        [Command("contest close submissions")]
        [Alias("contest end submissions")]
        [Summary("Closes the contest for submissions and starts a voting period"), Remarks("contest close submissions [contest title]")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        public async Task CloseContest([Remainder] string contestTitle)
        {
            if (!_contestHandler.ContestTitleExists(contestTitle))
            {
                await ReplyAsync($"A contest with the name \"{contestTitle}\" does not exist.");

                var allContestTitles = _contestHandler.GetStoredContestNames();
                var closeMatches = allContestTitles.FindClosestMatch(contestTitle, 6);

                if (closeMatches.Length is 0)
                {
                    await ReplyAsync($":bulb: Did you mean: \n\t• `{string.Join("`\n\t• `", closeMatches)}`");
                }

                return;
            }

            var contest = _contestHandler.GetContestByTitle(contestTitle);

            if (contest.Submissions.Count == 0)
            {
                _contestHandler.DeleteContest(contest);
                await ReplyAsync("Since there were no submissions in this contest, it has been deleted.");
                return;
            }

            var embedBuilder = GetVotingEmbedBuilder(contest);
            var votingMessage = await ReplyAsync("", embed: embedBuilder.Build());

            await Context.Message.DeleteAsync();

            await votingMessage.AddReactionAsync(new Emoji("❌"));

            foreach (var submission in contest.Submissions)
            {
                var emoji = new Emoji(submission.EmojiRawUnicode);
                await votingMessage.AddReactionAsync(emoji);
            }

            _contestHandler.StartVotingPeriod(contest, votingMessage.Id);
        }

        [Command("contest submission delete")]
        [Alias("contest submission remove")]
        [Summary("Delete a submission from a contest"), Remarks("contest submission delete [@UserToRemove] [contest title]")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        public async Task DeleteSubmission(IGuildUser user, [Remainder] string contestTitle)
        {
            if (!_contestHandler.ContestTitleExists(contestTitle))
            {
                await ReplyAsync($"A contest with the name \"{contestTitle}\" does not exist.");
                return;
            }

            var contest = _contestHandler.GetContestByTitle(contestTitle);
            var submission = _contestHandler.GetContestSubmissionFromUserId(contest, user.Id);

            if (submission == default)
            {
                await ReplyAsync($"{user.Mention} does not have a submission in that contest.");
                return;
            }

            contest = _contestHandler.DeleteContestSubmission(submission, contest);
            var contestChannel = Context.Guild.GetTextChannel(contest.SubmissionChannelId);

            if (contest.State is ContestState.VotingPeriod)
            {
                var votingMessage = (SocketUserMessage) await contestChannel.GetMessageAsync(contest.VotingMessageId);
                await votingMessage.ModifyAsync((MessageProperties props) => props.Embed = GetVotingEmbedBuilder(contest).Build());
                // Remove reactions and votes..
            }

            var message = await contestChannel.GetMessageAsync(submission.MessageId);
            await message?.DeleteAsync();

            await ReplyAsync($"Deleted submission by {user.Mention} in \"{contest.Title}\".");
        }

        [Command("contest delete")]
        [Summary("Delete a contest completely"), Remarks("contest delete [contest title]")]
        [RequireOwner]
        public async Task DeleteContest([Remainder] string contestTitle)
        {
            if (!_contestHandler.ContestTitleExists(contestTitle))
            {
                await ReplyAsync($"A contest with the name \"{contestTitle}\" does not exist.");
                return;
            }

            var contest = _contestHandler.GetContestByTitle(contestTitle);
            _contestHandler.DeleteContest(contest);

            await ReplyAsync($"Deleted contest \"{contest.Title}\".");
        }

        [Command("contest list all")]
        [Summary("Lists all inactive and active contests"), Remarks("contest list all")]
        public async Task ListAllContests()
        {
            await ListActiveContests();
            await ListInactiveContests();
        }

        [Command("contest list active")]
        [Summary("Lists active contests"), Remarks("contest list active")]
        public async Task ListActiveContests()
        {
            var activeContests = _contestHandler.GetContestsInState(ContestState.TakingSubmissions, ContestState.VotingPeriod);

            var embedBuilder = new EmbedBuilder()
                .WithTitle("Active contests")
                .WithColor(new Color(118, 196, 177));

            if (activeContests is null || activeContests.Length == 0)
            {
                embedBuilder.WithDescription("There are currently no active contests. You should send your contest ideas to `CalmEyE#8246` or `LeMorrow#8192` :wink:");
                await ReplyAsync("", embed: embedBuilder.Build());
                return;
            }

            foreach (var contest in activeContests)
            {
                embedBuilder.AddField(contest.Title,
                    $"Submissions: {contest.Submissions.Count}\n" +
                    $"Votes: {contest.GetAmountOfVoters()}\n" +
                    $"State: {contest.State}\n" +
                    $"Channel: {Context.Guild.GetTextChannel(contest.SubmissionChannelId).Mention}",
                    inline: true);
            }

            await ReplyAsync("", embed: embedBuilder.Build());
        }

        [Command("contest list inactive")]
        [Alias("contest list ended")]
        [Summary("Lists inactive contests"), Remarks("contest list inactive")]
        public async Task ListInactiveContests()
        {
            var inactiveContests = _contestHandler.GetContestsInState(ContestState.Ended);

            var embedBuilder = new EmbedBuilder()
                .WithTitle("Inactive contests")
                .WithColor(new Color(118, 196, 177));

            if (inactiveContests is null || inactiveContests.Length == 0)
            {
                embedBuilder.WithDescription("There are currently no inactive contests.");
                await ReplyAsync("", embed: embedBuilder.Build());
                return;
            }

            foreach (var contest in inactiveContests)
            {
                embedBuilder.AddField(contest.Title, 
                    $"Submissions: {contest.Submissions.Count}\n" +
                    $"Votes: {contest.GetAmountOfVoters()}\n" +
                    $"Date closed: {contest.EndDateUtc.ToLocalTime().ToString("dd-MM-yyyy")}\n" +
                    $"State: {contest.State}\n" +
                    $"Channel: {Context.Guild.GetTextChannel(contest.SubmissionChannelId).Mention}",
                    inline: true);
            }

            await ReplyAsync("", embed: embedBuilder.Build());
        }

        private EmbedBuilder GetVotingEmbedBuilder(Contest contest)
        {
            var endTime = DateTime.Now.AddDays(2);

            var embedBuilder = new EmbedBuilder()
                .WithTitle($"Who should win the contest \"{contest.Title}\"?")
                .WithDescription(
                    "Vote using the reactions below!\n\n" +
                    "• The contestants have been given random emojis.\n" +
                    "• Click the link below a submission to look at it again.\n" +
                    "• You are only able to vote for one person at a time (not yourself).\n" +
                    "• The voting is anynonymous, the bot will remove your reaction when it's counted.\n" +
                    "• To clear your vote, react with :x:\n\n" +
                    $"*The voting will close on {endTime.ToString("dddd, dd MMMM")} at {endTime.ToString("HH:mm")}.*")
                .WithColor(new Color(118, 196, 177))
                .WithFooter("0 votes"); // to be determined programatically

            _contestHandler.RandomizeContestSubmissionEmojis(contest);

            foreach (var submission in contest.Submissions)
            {
                var contestant = Context.Guild.GetUser(submission.AuthorId);
                embedBuilder.AddField($"{contestant.Nickname ?? contestant.Username} ({submission.EmojiRawUnicode})",
                    $"[See submission](https://discordapp.com/channels/{Context.Guild.Id}/{contest.SubmissionChannelId}/{submission.MessageId})",
                    inline: true);
            }

            return embedBuilder;
        }
    }
}
