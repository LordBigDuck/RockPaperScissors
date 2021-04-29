using System;
using System.Collections.Generic;
using System.Linq;

using FluentResults;

using RockPaperCisor.Domain.Domain.Enums;

namespace RockPaperCisor.Domain.Domain
{
    public class Match
    {
        public Guid Id { get; set; }

        public Player Player1 { get; private set; }

        public Player Player2 { get; private set; }

        public GameState State { get; private set; }

        public IReadOnlyList<Round> Rounds { get => _rounds.ToList().AsReadOnly(); }

        private readonly ICollection<Round> _rounds = new List<Round>();

        private Match(Player player1, Player player2)
        {
            Player1 = player1;
            Player2 = player2;

            StartNewRound();
        }

        public static Result<Match> CreateMatch(Player firstPlayer, Player secondPlayer)
        {
            if (firstPlayer == null || secondPlayer == null)
            {
                return Result.Fail("Player should exist");
            }

            if (firstPlayer.IsIdentical(secondPlayer))
            {
                return Result.Fail("Players should be different");
            }

            var match = new Match(firstPlayer, secondPlayer);
            return Result.Ok(match);
        }

        private Result StartNewRound()
        {
            if (MatchIsFinished)
            {
                return Result.Fail("Match is finished");
            }

            _rounds.Add(new Round());
            return Result.Ok();
        }

        public Result<Player> ComputeWinner()
        {
            if (TotalOfRoundsPlayed < MustWinNumber)
            {
                return Result.Fail("Game is not ended");
            }

            if (Player1WonRounds >= MustWinNumber)
            {
                return Result.Ok(Player1);
            }

            if (Player2WonRounds >= MustWinNumber)
            {
                return Result.Ok(Player2);
            }

            if (IsTie)
            {
                return Result.Ok();
            }

            return Result.Fail("Game is not ended");
        }

        public Player GetWinner()
        {
            var roundResult = CurrentRound.Winner;
            if (roundResult == Winner.None)
            {
                return null;
            }

            if (roundResult == Winner.Player1)
            {
                return Player1;
            }

            return Player2;
        }

        public Result SetPlayerVote(Player player, Hand vote)
        {
            if (IsPlayer1(player))
            {
                CurrentRound.SetPlayer1Vote(vote);
                return Result.Ok();
            }

            if (IsPlayer2(player))
            {
                CurrentRound.SetPlayer2Vote(vote);
                return Result.Ok();
            }

            return Result.Fail("Player is not in the game");
        }

        private const uint MaxRound = 5;

        private readonly uint MustWinNumber = (MaxRound / 2) + 1;

        private int Player1WonRounds => _rounds.Where(r => r.Winner == Winner.Player1).Count();
        private int Player2WonRounds => _rounds.Where(r => r.Winner == Winner.Player2).Count();
        private int TotalOfRoundsPlayed => _rounds.Count();
        private bool IsTie => Player1WonRounds == Player2WonRounds && TotalOfRoundsPlayed >= MaxRound;
        private bool MatchIsFinished => State == GameState.Done || TotalOfRoundsPlayed >= MaxRound;
        private bool IsPlayer1(Player player) => Player1.Name == player.Name;
        private bool IsPlayer2(Player player) => Player2.Name == player.Name;
        private Round CurrentRound => _rounds.Last();
    }
}
