using Discord;
using Discord.Commands;
using Discord.Addons.Interactive;
using System.Threading.Tasks;
using SpeldesignBotCore.Contests;

namespace SpeldesignBotCore.Modules
{
    public class VotingSystemCommands : InteractiveBase<SocketCommandContext>
    {
        private static int _cooldoges;
        private static int _angryboyes;

        private readonly ContestHandler _reactionHandler;
        private readonly InteractiveService _interactiveService;

        public VotingSystemCommands(ContestHandler reactionHandler, InteractiveService interactiveService)
        {
            _reactionHandler = reactionHandler;
            _interactiveService = interactiveService;
        }

        [Command("new contest", RunMode = RunMode.Async)]
        [Summary("Sets up a new contest"), Remarks("new contest")]
        [RequireOwner]
        public async Task NewContestSetup()
        {
            Emote cooldoge = Emote.Parse("<a:cooldoge:480130089149792257>");
            Emote angryboye = Emote.Parse("<:angryboye:458218227181289473>");

            var message = await ReplyAsync("So now it would be like how many people entered the contest and you would reply with like a number");

            _reactionHandler.AddReactionAction(message.Id, cooldoge, Cooldoge);
            _reactionHandler.AddReactionAction(message.Id, angryboye, Angryboye);

            await message.AddReactionAsync(cooldoge);
            await message.AddReactionAsync(angryboye);

            var test = await NextMessageAsync();
            await ReplyAsync($"Wow so hopefully you entered a number which means {test.Content} people joined the contest!");

            //await _interactiveService.AddReactionCallback(message, )
        }

        private async Task Cooldoge(SocketCommandContext context)
        {
            _cooldoges++;
            await UpdateStatsMessage();
        }

        private async Task Angryboye(SocketCommandContext context)
        {
            _angryboyes++;
            await UpdateStatsMessage();
        }

        private async Task UpdateStatsMessage()
        {
            await ReplyAsync($"Cooldoges: {_cooldoges}\nAngryboyes: {_angryboyes}");
        }
    }
}
