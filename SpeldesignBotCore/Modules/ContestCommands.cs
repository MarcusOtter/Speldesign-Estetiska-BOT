using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using SpeldesignBotCore.Contests;
using SpeldesignBotCore.Entities;
using System.Linq;

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
        [Summary("Creates a new contest and starts it"), Remarks("contest create [#submissionChannel] [contest title]")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        public async Task NewContestSetup(ITextChannel contestSubmissionChannel, [Remainder] string contestTitle = "")
        {
            if (string.IsNullOrWhiteSpace(contestTitle))
            {
                await ReplyAsync("You must give the contest a title.");
                return;
            }

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

            var contest = new Contest(contestTitle, contestSubmissionChannel.Id);

            _contestHandler.SaveNewContest(contest);

            var embed = new EmbedBuilder()
                .WithTitle("🏆 New contest created! 🏆")
                .WithDescription($"**{contest.Title}**\nSend your contest submissions in {contestSubmissionChannel.Mention}.\n*Make sure that {Context.Client.CurrentUser.Mention} is online when you submit!*")
                .WithColor(new Color(118, 196, 177))
                .WithFooter($"🎁 Enter the contest to have a chance to win rewards once it closes 🎁");

            await ReplyAsync("", embed: embed.Build());

            await Context.Message.DeleteAsync();
        }

        [Command("contest end")]
        [Alias("contest stop", "contest close")]
        [Summary("Ends the contest and sends rewards to the winners"), Remarks("contest end [contest title]")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        public async Task EndContest([Remainder] string contestTitle)
        {
            if (!_contestHandler.ContestTitleExists(contestTitle))
            {
                await ReplyAsync($"A contest with the name \"{contestTitle}\" does not exist.");

                var allContestTitles = _contestHandler.GetAllContests().Select(x => x.Title).ToArray();
                var closeMatches = allContestTitles.FindClosestMatch(contestTitle, 6);

                if (closeMatches.Any())
                {
                    await ReplyAsync($"Did you mean: \n`{string.Join("`, `", closeMatches)}`");
                }

                return;
            }

            var contest = _contestHandler.GetContestByTitle(contestTitle);
            _contestHandler.CloseContest(contest);

            await ReplyAsync($"Closed contest \"{contest.Title}\".");

            // TODO: start voting period (2 days?)
            // enum with state, not started, taking submissions, voting period, ended
            // contest needs a voting message id and like.. store an emoji for every contest submission maybe lol
            await ReplyAsync(":tada: PRIZES FOR EVERYONE :tada: :trophy:");
        }

        [Command("contest delete")]
        [Summary("Delete a contest completely")]
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
            var contests = _contestHandler.GetAllContests();
            var activeContests = contests?.Where(x => x.IsActive);

            var embedBuilder = new EmbedBuilder()
                .WithTitle("Active contests")
                .WithColor(new Color(118, 196, 177));

            if (activeContests is null || activeContests.Count() == 0)
            {
                embedBuilder.WithDescription("There are currently no active contests. You should send your contest ideas to `CalmEyE#8246` or `LeMorrow#8192` :wink:");
                await ReplyAsync("", embed: embedBuilder.Build());
                return;
            }

            foreach (var contest in activeContests)
            {
                embedBuilder.AddField(contest.Title, $"Submissions: {contest.GetAmountOfSubmissions()}\nVotes: {contest.GetAmountOfVoters()}", inline: true);
            }

            await ReplyAsync("", embed: embedBuilder.Build());
        }

        [Command("contest list inactive")]
        [Summary("Lists inactive contests"), Remarks("contest list inactive")]
        public async Task ListInactiveContests()
        {
            var contests = _contestHandler.GetAllContests();
            var inactiveContests = contests?.Where(x => !x.IsActive);

            var embedBuilder = new EmbedBuilder()
                .WithTitle("Inactive contests")
                .WithColor(new Color(118, 196, 177));

            if (inactiveContests is null || inactiveContests.Count() == 0)
            {
                embedBuilder.WithDescription("There are currently no inactive contests.");
                await ReplyAsync("", embed: embedBuilder.Build());
                return;
            }

            foreach (var contest in inactiveContests)
            {
                embedBuilder.AddField(contest.Title, 
                    $"Submissions: {contest.GetAmountOfSubmissions()}\n" +
                    $"Votes: {contest.GetAmountOfVoters()}\n" +
                    $"Date closed: {contest.EndDateUtc.ToLocalTime().ToString("dd-MM-yyyy")}", 
                    inline: true);
            }

            await ReplyAsync("", embed: embedBuilder.Build());
        }
    }
}
