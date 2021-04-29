using System;

using RockPaperCisor.Domain.Domain.Enums;

namespace RockPaperCisor.Domain.Domain
{
    public class Round
    {
        public Guid Id { get; private set; }

        public RoundState State { get; set; }

        public Hand Player1Vote { get; private set; }

        public Hand Player2Vote { get; private set; }

        public Winner Winner => GetWinner();

        internal Round()
        {
            Id = Guid.NewGuid();
        }

        public void SetPlayer1Vote(Hand vote) => Player1Vote = vote;

        public void SetPlayer2Vote(Hand vote) => Player2Vote = vote;

        private Winner GetWinner()
        {
            var winningVote = Rules.GetWinningVote(Player1Vote, Player2Vote);

            if (winningVote == default)
            {
                return Winner.None;
            }
            else if (winningVote == Player1Vote)
            {
                return Winner.Player1;
            }
            return Winner.Player2;
        }
    }
}
