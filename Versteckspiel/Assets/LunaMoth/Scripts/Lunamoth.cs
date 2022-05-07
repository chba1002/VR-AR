using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lunamoth : MonoBehaviour
{
    private Animator lunamoth;
    private bool State1 = true;
    private bool State2 = false;

    void Start()
    {
        lunamoth = GetComponent<Animator>();
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            State1 =! State1;
            State2 = false;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            State2 =! State2;
            State1 = false;
        }
        if (lunamoth.GetCurrentAnimatorStateInfo(0).IsName("idle")&&(State1==true))
        {
            lunamoth.SetBool("takeoff", false);
            lunamoth.SetBool("landing", false);
        }
        if (Input.GetKeyDown(KeyCode.Space)&&(State1==true))
        {
            lunamoth.SetBool("idle", false);
            lunamoth.SetBool("takeoff", true);
            lunamoth.SetBool("landing", true);
            lunamoth.SetBool("fly", false);
            lunamoth.SetBool("flyleft", false);
            lunamoth.SetBool("flyright", false);
        }
        if (lunamoth.GetCurrentAnimatorStateInfo(0).IsName("idle2") && (State2 == true))
        {
            lunamoth.SetBool("takeoff2", false);
            lunamoth.SetBool("landing2", false);
        }
        if (Input.GetKeyDown(KeyCode.Space) && (State2 == true))
        {
            lunamoth.SetBool("idle2", false);
            lunamoth.SetBool("takeoff2", true);
            lunamoth.SetBool("landing2", true);
            lunamoth.SetBool("fly", false);
            lunamoth.SetBool("flyleft", false);
            lunamoth.SetBool("flyright", false);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            lunamoth.SetBool("flyleft", true);
            lunamoth.SetBool("fly", false);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            lunamoth.SetBool("flyright", true);
            lunamoth.SetBool("fly", false);
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            lunamoth.SetBool("fly", true);
            lunamoth.SetBool("flyleft", false);
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            lunamoth.SetBool("fly", true);
            lunamoth.SetBool("flyright", false);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            lunamoth.SetBool("walk", true);
            lunamoth.SetBool("idle", false);
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            lunamoth.SetBool("idle", true);
            lunamoth.SetBool("walk", false);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            lunamoth.SetBool("backward", true);
            lunamoth.SetBool("idle", false);
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            lunamoth.SetBool("backward", false);
            lunamoth.SetBool("idle", true);
        }
        if (Input.GetKeyDown(KeyCode.A)&&(State1==true))
        {
            lunamoth.SetBool("turnleft", true);
            lunamoth.SetBool("idle", false);
        }
        if (Input.GetKeyUp(KeyCode.A)&&(State1==true))
        {
            lunamoth.SetBool("idle", true);
            lunamoth.SetBool("turnleft", false);
        }
        if (Input.GetKeyDown(KeyCode.D)&&(State1==true))
        {
            lunamoth.SetBool("idle", false);
            lunamoth.SetBool("turnright", true);
        }
        if (Input.GetKeyUp(KeyCode.D)&&(State1==true))
        {
            lunamoth.SetBool("idle", true);
            lunamoth.SetBool("turnright", false);
        }
        if (Input.GetKeyDown(KeyCode.A) && (State2 == true))
        {
            lunamoth.SetBool("turnleft2", true);
            lunamoth.SetBool("idle2", false);
        }
        if (Input.GetKeyUp(KeyCode.A) && (State2 == true))
        {
            lunamoth.SetBool("idle2", true);
            lunamoth.SetBool("turnleft2", false);
        }
        if (Input.GetKeyDown(KeyCode.D) && (State2 == true))
        {
            lunamoth.SetBool("idle2", false);
            lunamoth.SetBool("turnright2", true);
        }
        if (Input.GetKeyUp(KeyCode.D) && (State2 == true))
        {
            lunamoth.SetBool("idle2", true);
            lunamoth.SetBool("turnright2", false);
        }
    }
}
