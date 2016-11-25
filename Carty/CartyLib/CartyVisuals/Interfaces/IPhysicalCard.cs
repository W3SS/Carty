﻿using System.Collections;
using UnityEngine;

namespace Carty.CartyVisuals
{
    /// <summary>
    /// Interface for customization of actual card outlook.
    /// </summary>
    public interface IPhysicalCard
    {
        /// <summary>
        /// Creates an object which physically represents the card.
        /// </summary>
        /// <returns>Created game object.</returns>
        GameObject CreatePhysicalCardObject();

        /// <summary>
        /// Applies a texture on the front of the card.
        /// </summary>
        /// <param name="physicalCardGO">Physical card game object. Created by CreatePhysicalCardObject.</param>
        /// <param name="frontTexture">Texture to apply.</param>
        void SetCardFront(GameObject physicalCardGO, Texture frontTexture);

        /// <summary>
        /// Applies a texture on the back of the card.
        /// </summary>
        /// <param name="physicalCardGO">Physical card game object. Created by CreatePhysicalCardObject.</param>
        /// <param name="backTexture">Texture to apply.</param>
        void SetCardBack(GameObject physicalCardGO, Texture backTexture);

        /// <summary>
        /// Animate destroying of a card.
        /// The result of the destruction should be invisible, completely transparent or destroyed game object.
        /// </summary>
        /// <param name="physicalCardGO">Physical card game object. Created by CreatePhysicalCardObject.</param>
        /// <returns>Coroutine which ends when the card was destroyed.</returns>
        IEnumerator DestroyPhysicalCard(GameObject physicalCardGO);

    }
}
