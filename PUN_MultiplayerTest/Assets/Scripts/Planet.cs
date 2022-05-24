using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MarchingCubes.MarchingCubeChunkHandler
{

    public const int MESH_COLLIDER_LAYER = 7;

    protected const int MESH_COLLIDER_LAYERMASK = 1 << MESH_COLLIDER_LAYER;

    protected float gravity = -1f;

    public float Gravity => gravity;

    public static Planet PlanetRef { get; private set; }

    public override void OnAwake()
    {
        if (PlanetRef != null)
        {
            Debug.LogError("There should only be one planet in the scene!", this);
        }
        PlanetRef = this;
    }

    protected override void OnPlanetDone()
    {
        NetworkGameManager networkGameManager = GameObject.FindObjectOfType<NetworkGameManager>();
        Transform player = networkGameManager.InstantiatePlayer().transform;
        player.position = GetPlayerStartPosition();

        if(PhotonNetwork.IsMasterClient || !PhotonNetwork.IsConnected)
        {
            OnPlanetDoneOnMaster();
        }
    }

    protected void OnPlanetDoneOnMaster()
    {
        SpawnPortal();
    }

    protected void SpawnPortal()
    {
        PortalCreator portal = GameObject.FindObjectOfType<PortalCreator>();
        Transform t;
        Vector3 pos = GetStartPosInYDirection(false);
        Quaternion rotation = PlanetCharacterController.GetRotationToPlanetGravity(pos, transform.position);
        if (PhotonNetwork.IsConnected)
            t = PhotonNetwork.Instantiate(portal.prefab.name, pos, rotation).transform;
        else
        {
            t = Instantiate(portal.prefab).transform;
            t.position = pos;
            t.rotation = rotation;
        }
    }

    public Vector3 GetPlayerStartPosition()
    {
        return GetStartPosInYDirection(true,3);
    }


    public Vector3 GetStartPosInYDirection(bool up, float distanceFromSurface = 0)
    {
        float dir = up ? 1 : -1;

        float radius = Radius;
        Vector2 playerStartPos = UnityEngine.Random.insideUnitCircle * radius * 2;
        Vector3 startPos = new Vector3(playerStartPos.x, radius * dir * 4, playerStartPos.y);
        RaycastHit hit;
        Ray r = new Ray(transform.position + startPos, -startPos);
        if (Physics.Raycast(r, out hit, radius * 4, MESH_COLLIDER_LAYERMASK))
        {
            startPos = hit.point + startPos.normalized * distanceFromSurface;
            return startPos;
        }
        else
            throw new Exception("Should have collided!");
    }



}
