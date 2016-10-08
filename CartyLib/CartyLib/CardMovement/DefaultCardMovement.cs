﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CartyLib.CardsComponenets
{
    /// <summary>
    /// Default implementation of ICardMovement.
    /// </summary>
    class DefaultCardMovement : ICardMovement
    {
        private static float MaxSpeed = 7.0f;
        private static float MinSpeed = 3.0f;

        public IEnumerator Flip(CanBeMoved card)
        {
            float t = 0.0f;

            float difference = Quaternion.Angle(card.transform.rotation, CanBeMoved.Flipped_On);
            bool initialy_flipped_on = difference < 5.0f;

            Quaternion initial_state = initialy_flipped_on ? CanBeMoved.Flipped_On : CanBeMoved.Flipped_Off;
            Quaternion wanted_state = (!initialy_flipped_on) ? CanBeMoved.Flipped_On : CanBeMoved.Flipped_Off;

            while (true)
            {
                if (t >= 1.0f) break;

                card.transform.rotation = Quaternion.Lerp(initial_state, wanted_state, t);
                t += Time.deltaTime;

                yield return null;
            }
        }

        public IEnumerator FlipInstantly(CanBeMoved card)
        {
            card.transform.rotation = Quaternion.Angle(card.transform.rotation, CanBeMoved.Flipped_On) < 5.0f ? 
                CanBeMoved.Flipped_Off : CanBeMoved.Flipped_On;
            yield break;
        }

        public IEnumerator Move(CanBeMoved card, Vector3 position)
        {
            Vector3 previous_position = card.transform.position;
            Vector3 middle = Vector3.Lerp(position, previous_position, 0.5f);
            float distance = Vector3.Distance(previous_position, position);

            float speed_modifier = 2.0f;

            if (distance < MinSpeed)
            {
                float t = 0.0f;
                while (true)
                {
                    if (t >= 1.0f) break;

                    card.transform.position = Vector3.Lerp(previous_position, position, t);
                    t += Time.deltaTime * speed_modifier;

                    yield return null;
                }
            }
            else
            {
                float speed = 0.0f;

                while (true)
                {
                    if (position == card.transform.position) break;

                    float t = 1.0f - (Vector3.Distance(card.transform.position, middle) /
                        (Vector3.Distance(position, previous_position) * 0.5f));
                    speed = Mathf.Lerp(MinSpeed, MaxSpeed, t);
                    card.transform.position = Vector3.MoveTowards(card.transform.position,
                        position, speed * Time.deltaTime);

                    yield return null;
                }
            }
        }

        public IEnumerator MoveInstantly(CanBeMoved card, Vector3 position)
        {
            card.transform.position = position;
            yield break;
        }

        public IEnumerator Rotate(CanBeMoved card, Quaternion rotation)
        {
            float t = 0.0f;
            float speed_modifier = 3.0f;

            Quaternion initial_state = card.transform.rotation;
            Quaternion wanted_state = rotation;

            while (true)
            {
                if (t >= 1.0f) break;

                card.transform.rotation = Quaternion.Lerp(initial_state, wanted_state, t);
                t += speed_modifier * Time.deltaTime;

                yield return null;
            }
        }

        public IEnumerator RotateInstantly(CanBeMoved card, Quaternion rotation)
        {
            card.transform.rotation = rotation;
            yield break;
        }
    }
}
