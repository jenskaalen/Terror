using System.Runtime.Serialization.Formatters;
using UnityEngine;
using System.Collections;

public class Tets : MonoBehaviour
{
    private Animator _animator;
    private int _walkHash, _idleHash, _strafeLeft, _strafeRight, _lastPlayedAnimation;

    // Use this for initialization
    void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        _walkHash = Animator.StringToHash("walking");
        _idleHash = Animator.StringToHash("idle");
        _strafeLeft = Animator.StringToHash("left_strafe_walking");
        _strafeRight = Animator.StringToHash("right_strafe_walking");
    }

    // Update is called once per frame
    void Update()
    {
        float hMove = Input.GetAxis("Horizontal");
        float vMove = Input.GetAxis("Vertical");

        if (hMove != 0 || vMove != 0)
        {
            //if (hMove == 0)
            //    _animator.Play(_strafeLeft);
            if (vMove == 0)
            {
                if (hMove < 0)
                    _animator.Play(_strafeLeft);
                else
                    _animator.Play(_strafeRight);
            }
            else
                _animator.Play(_walkHash);
        }
        else
        {
            _animator.CrossFade(_idleHash, 0.1f);
        }
    }

    [RPC]
    public void PlayAnimation(int hash)
    {
        _animator.Play(hash);
    }
}
