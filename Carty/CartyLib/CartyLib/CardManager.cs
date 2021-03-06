﻿using Carty.CartyLib.Internals.CardsComponents;
using Carty.CartyVisuals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Carty.CartyLib.Internals
{

    public class CardManagerCardInfo
    {
        public GameObject Card = null;
        public ICardType CardType = null;

        public bool IsSpell = false;
        public ISpell Spell = null;
    }

    /// <summary>
    /// Class responsible for creating and keeping track of cards.
    /// Through reflection gathers all user created cards from application assemblies.
    /// </summary>
    public class CardManager
    {
        /// <summary>
        /// Map between unique card type id and card type.
        /// </summary>
        private Dictionary<string, Type> TypeMapping { get; set; }

        public CardManager() {
            TypeMapping = new Dictionary<string, Type>();
            AllCards = new List<CardManagerCardInfo>();
        }

        /// <summary>
        /// All cards created for this match.
        /// </summary>
        public List<CardManagerCardInfo> AllCards { get; private set; }

        /// <summary>
        /// Immediately destroys all remaining cards.
        /// </summary>
        public void CleanUp()
        {
            for(int i=0; i < AllCards.Count; i++)
            {
                DestroyCard(AllCards[i].Card);
            }

            AllCards.Clear();
        }

        /// <summary>
        /// Initializes the card manager. 
        /// Scan all application assemblies for MonoBehaviours implementing CartyLib.ICardType.
        /// </summary>
        public void Initialize()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type[] types = null;

                try
                {
                    types = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    Debug.LogError("Failed to load types from: " + assembly.FullName);
                    foreach (Exception loadEx in ex.LoaderExceptions)
                        Debug.LogException(loadEx);
                }

                if (types == null)
                    continue;

                foreach (Type type in types)
                {
                    if(type.GetInterfaces().Contains(typeof(ICardType)) && type.BaseType == typeof(MonoBehaviour))
                    {
                        GameObject obj = new GameObject();

                        string id = (obj.AddComponent(type) as ICardType).GetInfo().UniqueCardTypeId;
                        TypeMapping.Add(id, type);
                        GameObject.Destroy(obj);
                    }
                }
            }
        }

        /// <summary>
        /// Assembles a card.
        /// </summary>
        /// <param name="uniqueCardTypeId">Unique card type id. See CardInfo.UniqueCardTypeId.</param>
        /// <param name="player">Whether the card is owned by player.</param>
        /// <returns>Created card game object.</returns>
        public GameObject CreateCard(string uniqueCardTypeId, bool player)
        {
            GameObject card = new GameObject("card");

            GameObject detachHandle = new GameObject("handle");
            detachHandle.transform.parent = card.transform;            

            card.AddComponent<CanBeDetached>();
            card.AddComponent<CanBeOwned>().PlayerOwned = player;
            card.AddComponent<CanBeMoved>();
            card.AddComponent<CanBeMousedOver>();
            card.AddComponent<CanBeInHand>();
            card.AddComponent<HasOutline>();
            card.AddComponent<CanBeHighlighted>();
            card.AddComponent<CanBeDragged>();
            card.AddComponent<CanBeInteractedWith>();
            card.AddComponent<HasCost>();
            var physicalCard = card.AddComponent<HasPhysicalCard>();

            VisualManager.Instance.PhysicalCard.AttachPhysicalCardCollider(card);

            CardManagerCardInfo cardInfo = new CardManagerCardInfo();
            cardInfo.Card = card;

            // Find in type mapping
            Type cardType;
            if (TypeMapping.TryGetValue(uniqueCardTypeId, out cardType))
            {
                var iCard = card.AddComponent(cardType) as ICardType;
                cardInfo.CardType = iCard;

                if (cardType.GetInterfaces().Contains(typeof(ISpell)))
                {
                    cardInfo.IsSpell = true;
                    cardInfo.Spell = iCard as ISpell;
                }

                // Attempt to apply front and back texture
                ApplyFrontTexture(physicalCard, iCard.GetInfo());
                ApplyBackTexture(physicalCard, player);
            }

            AllCards.Add(cardInfo);

            return card;
        }

        /// <summary>
        /// Immediately destroys the card.
        /// </summary>
        /// <param name="card"></param>
        public void DestroyCard(GameObject card)
        {
            AllCards.RemoveAll(x => x.Card == card);
            GameObject.Destroy(card);
        }

        /// <summary>
        /// Find info about a card, searching in all created cards.
        /// </summary>
        /// <param name="card">Card to find.</param>
        /// <returns>CardManagerCardInfo wrapper around the card.</returns>
        public CardManagerCardInfo FindCardInfo(GameObject card)
        {
            return AllCards.Find(x => x.Card == card);
        }

        private void ApplyBackTexture(HasPhysicalCard physicalCard, bool player)
        {
            var backTextureName = player ? GameManager.Instance.MatchInfo.PlayerCardBackTexture :
                GameManager.Instance.MatchInfo.EnemyCardBackTexture;

            var textureFolder = VisualManager.Instance.CardTexturesPath;
            if (textureFolder.EndsWith("\\") == false) textureFolder += "\\";

            var backTexture = Resources.Load(textureFolder + backTextureName) as Texture;
            if (backTexture == null)
            {
                backTexture = Resources.Load(backTextureName) as Texture;
            }

            if (backTexture != null)
            {
                VisualManager.Instance.PhysicalCard.SetCardBack(physicalCard.PhysicalCardGO, backTexture);
            }
        }

        private void ApplyFrontTexture(HasPhysicalCard physicalCard, CardInfo cardInfo)
        {
            var frontTextureName = cardInfo.CardFrontTexture;

            var textureFolder = VisualManager.Instance.CardTexturesPath;
            if (textureFolder.EndsWith("\\") == false) textureFolder += "\\";

            var frontTexture = Resources.Load(textureFolder + frontTextureName) as Texture;
            if (frontTexture == null)
            {
                frontTexture = Resources.Load(frontTextureName) as Texture;
            }

            if (frontTexture != null)
            {
                VisualManager.Instance.PhysicalCard.SetCardFront(physicalCard.PhysicalCardGO, frontTexture);
            }
        }
    }
}
