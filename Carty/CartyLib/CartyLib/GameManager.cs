﻿using Carty.CartyLib.Internals;
using Carty.CartyLib.Internals.BoardComponents;
using UnityEngine;

namespace Carty.CartyLib
{
    /// <summary>
    /// Facade singleton for the CartyLib inner-workings.
    /// Initializes all necessary subsystems and starts a match of Carty game.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        /// <summary>
        /// Singleton accessor.
        /// </summary>
        public static GameManager Instance { get; set; }

        /// <summary>
        /// Hand of the player.
        /// </summary>
        public Hand PlayerHand { get; private set; }

        /// <summary>
        /// Hand of the enemy.
        /// </summary>
        public Hand EnemyHand { get; private set; }

        /// <summary>
        /// Deck of the player.
        /// </summary>
        public Deck PlayerDeck { get; private set; }

        /// <summary>
        /// Deck of the enemy.
        /// </summary>
        public Deck EnemyDeck { get; private set; }

        /// <summary>
        /// How many turns player has played currently.
        /// </summary>
        [HideInInspector]
        public int PlayerTurnCount;

        /// <summary>
        /// How many turns enemy has played currently.
        /// </summary>
        [HideInInspector]
        public int EnemyTurnCount;

        /// <summary>
        /// Game logic oriented settings and rules.
        /// To modify them simply change the values inside.
        /// </summary>
        public GameSettings Settings { get; private set; }

        /// <summary>
        /// Match info passed in StartMatch call for later use.
        /// </summary>
        public MatchInfo MatchInfo { get; private set; }


        private GameQueueManager _queueManager;
        /// <summary>
        /// Game queue manager instance.
        /// See GameQueueManager.
        /// </summary>
        public GameQueueManager GameQueue {
            get
            {
                if(_queueManager == null)
                {
                    _queueManager = new GameQueueManager();
                }

                return _queueManager;
            }
        }

        private CardManager _cardManager;
        /// <summary>
        /// Card manager instance.
        /// See CardManager.
        /// </summary>
        public CardManager CardManager
        {
            get
            {
                if (_cardManager == null)
                {
                    _cardManager = new CardManager();
                }

                return _cardManager;
            }
        }

        void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }

            CardManager.Initialize();
            Settings = new GameSettings();
            GameQueue.Start();
        }

        private void CreateBoardObjects()
        {
            if (PlayerHand == null) PlayerHand = Hand.CreateHand(true);
            if (EnemyHand == null) EnemyHand = Hand.CreateHand(false);
            if (PlayerDeck == null) PlayerDeck = Deck.CreateDeck(true);
            if (EnemyDeck == null)  EnemyDeck = Deck.CreateDeck(false);
        }

        /// <summary>
        /// Starts a match. Creates all necessary game objects and cards.
        /// </summary>
        /// <param name="matchInfo">Information about initial state of the match. See MatchInfo.</param>
        public void StartMatch(MatchInfo matchInfo)
        {
            CreateBoardObjects();
            PlayerDeck.FillWithCards(matchInfo.PlayerDeckCards);
            EnemyDeck.FillWithCards(matchInfo.EnemyDeckCards);
            PlayerHand.FillWithCards(matchInfo.PlayerStartingHandCards);
            EnemyHand.FillWithCards(matchInfo.EnemyStartingHandCards);

            MatchInfo = matchInfo;

            if(matchInfo.PlayerAmountOfCardDrawBeforeGame > 0)
            {
                PlayerHand.ImmidiatelyTakeCardsFromDeck(PlayerDeck, matchInfo.PlayerAmountOfCardDrawBeforeGame);
            }

            if (matchInfo.EnemyAmountOfCardDrawBeforeGame > 0)
            {
                EnemyHand.ImmidiatelyTakeCardsFromDeck(EnemyDeck, matchInfo.EnemyAmountOfCardDrawBeforeGame);
            }

            if (matchInfo.PlayerGoesFirst)
            {
                StartPlayerTurn();
            }
            else
            {
                StartEnemyTurn();
            }
        }

        /// <summary>
        /// Immediately cleans-up the match, destroying all cards in hands, decks and boards. 
        /// Prepares everything for new match.
        /// </summary>
        public void CleanUpMatch()
        {
            GameQueue.CleanUp();

            PlayerHand.CleanUp();
            EnemyHand.CleanUp();

            PlayerDeck.CleanUp();
            EnemyDeck.CleanUp();

            CardManager.CleanUp();

            MatchInfo = null;
            PlayerTurnCount = 0;
            EnemyTurnCount = 0;
        }

        /// <summary>
        /// Enqueues a start of a player turn.
        /// A card(s) is drawn for player, turn start event is called and player is given control over his hand and board.
        /// </summary>
        public void StartPlayerTurn()
        {
            PlayerTurnCount++;
            int cardsToDraw = Settings.TurnStartCardDrawSetting(PlayerTurnCount, true, MatchInfo.PlayerGoesFirst);
            GameQueue.PlayerDrawCards(cardsToDraw);
        }

        /// <summary>
        ///  Enqueues a start of a enemy turn.
        ///  A card(s) is drawn for player, turn start event is called and AI takes over.
        /// </summary>
        public void StartEnemyTurn()
        {
            EnemyTurnCount++;
        }

        /// <summary>
        /// Enables or disables interaction with all player cards. See CanBeInteractedWith.
        /// </summary>
        /// <param name="enable">Whether to enable or disable interaction.</param>
        public void EnableInteraction(bool enable)
        {
            PlayerHand.EnableInteraction(enable);
        }


    }
}
