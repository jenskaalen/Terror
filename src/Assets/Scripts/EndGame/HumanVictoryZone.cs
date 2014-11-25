using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Scripts.EndGame
{
    public class HumanVictoryZone : MonoBehaviour
    {
        private readonly List<HumanController> _humans = new List<HumanController>();
        private GameInfo _gameInfo;

        void Awake()
        {
            //TODO: get reference to a global static variable instead
            _gameInfo = FindObjectOfType<GameInfo>();
        }

        void OnTriggerEnter(Collider enteredCollider)
        {
            if (enteredCollider.tag == "human")
            {
                var humanController = enteredCollider.GetComponent<HumanController>();

                if (_humans.All(controller => controller !=  humanController ))
                {
                    _humans.Add(humanController);
                }

                CheckForVictory();
            }
        }

        void OnTriggerExit(Collider exitedCollider)
        {
            if (exitedCollider.tag == "human")
            {
                var humanController = exitedCollider.GetComponent<HumanController>();

                if (_humans.Contains(humanController))
                {
                    _humans.Remove(humanController);
                }
            }
        }

        void CheckForVictory()
        {
            var allHumans = _gameInfo.PlayerInfos
                .Where(info => info.PlayerControl is HumanController)
                .Select(info => info.PlayerControl as HumanController)
                .ToList();

            if (allHumans.All(control => _humans.Contains(control))) ;
            {
                EndGameHandling.HumanVictory();
            }
        }
    }
}
