using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Util
{
    public static class UnityExtensions
    {
        public static Collider ForwardRay(Transform forwardForm, float distance)
        {
            RaycastHit hit;
            Ray ray = new Ray(forwardForm.position, forwardForm.forward);

            if (Physics.Raycast(ray, out hit, distance))
            {

                return hit.collider;
            }

            return null;
        }
    }
}
