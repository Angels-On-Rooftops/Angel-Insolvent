using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Libs
{
    static class SpecialObjects
    {
        public static bool IsCharacter(GameObject potentialCharacter)
        {
            return potentialCharacter.TryGetComponent<CharacterMovement>(out _);
        }

        public static bool IsCharacter<T>(T potentialCharacter)
            where T : Component
        {
            return IsCharacter(potentialCharacter.gameObject);
        }
    }
}
