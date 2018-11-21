using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class SpinMe : MonoBehaviour
    {

        [SerializeField] float xRotationsPerMinute = 1f;
        [SerializeField] float yRotationsPerMinute = 1f;
        [SerializeField] float zRotationsPerMinute = 1f;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            // xDegreesPerFrame = TIme.DeltaTIme, 60, 360, xRotationsPerMinute
            // degrees frame^-1 = seconds frame^-1 / seconds minute^-1, degrees rotation^-1 * rotation minute^-1
            // degrees frame^-1 = frame^-1 minute * degrees rotation^-1 * rotation minute^-1
            // degrees frame^-1 = frame^-1 degrees
            // degrees / frame (is the same like writing ^-1)

            float xDegreesPerFrame = Time.deltaTime / 60 * 360 * xRotationsPerMinute;
            transform.RotateAround(transform.position, transform.right, xDegreesPerFrame);

            float yDegreesPerFrame = Time.deltaTime / 60 * 360 * yRotationsPerMinute;
            transform.RotateAround(transform.position, transform.up, yDegreesPerFrame);

            float zDegreesPerFrame = Time.deltaTime / 60 * 360 * zRotationsPerMinute;
            transform.RotateAround(transform.position, transform.forward, zDegreesPerFrame);
        }
    }
}
