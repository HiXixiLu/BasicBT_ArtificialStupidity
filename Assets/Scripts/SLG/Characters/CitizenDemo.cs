﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CitizenDemo : CharacterBase
{
    private void Awake()
    {
        Health = CharacterLimits.HealthLimit;
        MovementScaleSetter = CharacterLimits.ArcherMovementScale;
        NameSetter = CharacterLimits.citizenName;

        base.Awake();
    }

    public override void handleInteraction(characterInteraction action, int value)
    {
        switch (action)
        {
            case characterInteraction.BE_ATTACKED:
                beAttacked(value);
                break;
            case characterInteraction.BE_RESCUED:
                beRescued(value);
                break;
            default:
                break;
        }
    }

    
}
