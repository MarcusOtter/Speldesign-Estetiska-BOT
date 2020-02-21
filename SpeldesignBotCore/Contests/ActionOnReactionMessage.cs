using Discord;
using Discord.Commands;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpeldesignBotCore.Contests
{
    public class ActionOnReactionMessage
    {
        public readonly ulong MessageId;
        public bool HasActionFor(Emote emote) => _actions.ContainsKey(emote.Id);

        public delegate Task ReactionAction(SocketCommandContext context);

        private readonly Dictionary<ulong, ReactionAction> _actions;

        public ActionOnReactionMessage(ulong messageId, params (Emote emote, ReactionAction action)[] actions)
        {
            MessageId = messageId;
            _actions = new Dictionary<ulong, ReactionAction>(actions.Length);
            for (int i = 0; i < actions.Length; i++)
            {
                _actions.Add(actions[i].emote.Id, actions[i].action);
            }
        }

        public async Task RunActionAsync(Emote emote, SocketCommandContext context)
        {
            await _actions[emote.Id](context);
        }

        public void AddAction(Emote emote, ReactionAction action)
        {
            if (_actions.ContainsKey(emote.Id)) { throw new System.ArgumentException($"This message already has an action for {emote.Name}."); }
            _actions.Add(emote.Id, action);
        }

        public void RemoveAction(Emote emote)
        {
            _actions.Remove(emote.Id);
        }
    }
}
