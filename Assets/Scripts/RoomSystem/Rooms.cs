﻿using System;
using UnityEngine;

namespace Assets.Scripts.RoomSystem
{
    [Serializable]
    public enum Room
    {
        MainMenu,

        Loading,

        Vault,

        CityHall,

        Cantata,

        [InspectorName("AmPower")]
        AmPower,

        Channel440,

        YCorp,
    }

    [Serializable]
    public enum InitialRoomPoint
    {
        [InspectorName("Inside/Vault Start")]
        VaultStart,

        [InspectorName("Inside/City Hall")]
        CityHallInside,

        [InspectorName("Inside/AmPower")]
        AmPowerInside,

        [InspectorName("Inside/Channel 440")]
        Channel440Inside,

        [InspectorName("Inside/Y Corp")]
        YCorpInside,

        [InspectorName("Outside/City Hall")]
        CityHallOutside,

        [InspectorName("Outside/AmPower")]
        AmPowerOutside,

        [InspectorName("Outside/Channel 440")]
        Channel440Outside,

        [InspectorName("Outside/Y Corp")]
        YCorpOutside,
    }
}
