using System;
using System.Collections.Generic;
using System.Linq;

using FluentResults;

using RockPaperCisor.Domain.Domain;
using RockPaperCisor.Domain.Domain.Enums;

namespace ConsoleApp
{
    public class Program
    {
        private static Player player1;
        private static Player player2;

        static void Main(string[] args)
        {
            ShowPossibleVotes();
            SetPlayers();
            var match = StartGame();

            while(match.State != GameState.Done)
            {
                Console.WriteLine("");
                var vote1 = GetPlayerVote(player1);
                SetPlayerVote(match, player1, vote1);

                var vote2 = GetPlayerVote(player2);
                SetPlayerVote(match, player2, vote2);

                match.EndPlayingRound();
                var winner = GetLastRoundWinner(match);
                ShowRoundWinner(winner);

                var matchWinner = match.ComputeWinner();
                if (matchWinner.IsSuccess)
                {
                    ShowMatchWinner(matchWinner.Value);
                }

                match.StartNewRound();
            }
        }

        private static void ShowPossibleVotes()
        {
            Console.WriteLine("Rock: 1 | Paper: 2 | Cisor: 3");
            Console.WriteLine("");
        }

        private static void ShowMatchWinner(Player player)
        {
            Console.WriteLine("");
            if (player == null)
            {
                Console.WriteLine($"The match is a tie");
            }
            else
            {
                Console.WriteLine($"{player.Name} has won the MATCH !!!");
            }
        }

        private static void ShowRoundWinner(Player player)
        {
            Console.WriteLine("");
            if (player == null)
            {
                Console.WriteLine($"This is a tie");
            }
            else
            {
                Console.WriteLine($"{player.Name} has won the round");
            }
        }

        private static Hand GetPlayerVote(Player player)
        {
            Console.WriteLine($"{player.Name} vote: ");
            var input = Console.ReadLine();
            Hand vote = null;
            var value = int.Parse(input);
            if (value == 1)
            {
                vote = new Rock();
            }
            else if (value == 2)
            {
                vote = new Paper();
            }
            else if (value == 3)
            {
                vote = new Scissor();
            }

            return vote;
        }

        private static Player GetLastRoundWinner(Match match)
        {
            var round = match.Rounds.LastOrDefault();
            var winner = round.Winner;

            if (winner == Winner.Player1)
            {
                return player1;
            }
            else if (winner == Winner.Player2)
            {
                return player2;
            }

            return null;
        }

        private static void SetPlayerVote(Match match, Player player, Hand vote)
        {
            var round = match.Rounds.LastOrDefault();
            match.SetPlayerVote(player, vote);
        }

        private static void SetPlayers()
        {
            player1 = SetPlayer(1);
            player2 = SetPlayer(2);
        }

        private static Player SetPlayer(int playerNumber)
        {
            var player = GetPlayer(playerNumber);
            while (player == null)
            {
                Console.WriteLine("Player cannot be null");
                player = GetPlayer(playerNumber);
            }
            return player;
        }

        private static Player GetPlayer(int playerNumber)
        {
            Console.WriteLine($"Player {playerNumber}: ");
            var input = Console.ReadLine();
            if (input == null)
            {
                return null;
            }
            return new Player(input);
        }

        private static Match StartGame()
        {
            var matchResult = Match.Create(player1, player2);
            if (matchResult.IsFailed)
            {
                DisplayErrors(matchResult.Errors);
                return default;
            }

            var match = matchResult.Value;
            match.StartNewRound();

            return match;
        }

        private static void DisplayErrors(ICollection<Error> errors)
        {
            foreach (var error in errors)
            {
                Console.WriteLine("");
                Console.WriteLine(error.Message.ToString());
            }
        }
    }
}
