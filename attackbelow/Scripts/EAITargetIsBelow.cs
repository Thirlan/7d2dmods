using System;
using UnityEngine;

public class EAITargetIsBelow : EAIBase
{
    private EntityAlive entityTarget;
    private int waitCounter;
    private int itemActionType;
    private int attackPeriod;
    private int attackPeriodMax;
	private float belowOffset;
	private float minimumProximity;

    public EAITargetIsBelow()
    {
        this.waitCounter = 60;
        this.attackPeriodMax = 20;
		this.belowOffset = 1f;
		this.minimumProximity = 8f;
    }

    public override void SetPar1(string _itemActionType)
    {
        itemActionType = int.Parse(_itemActionType);
    }

    public override void SetPar2(string _attackPeriodMax)
    {
        this.attackPeriodMax = int.Parse(_attackPeriodMax) * 20;
    }

    public bool IsBelow(float zEntity, float zTarget, float distanceSq)
    {
		if(((zEntity - belowOffset) > zTarget) && distanceSq < this.minimumProximity)
		{
			Debug.Log("zEntity: "+(zEntity - belowOffset)+" / zTarget: "+zTarget+" / distanceSq: "+distanceSq);
			return true;
		}
		else
		{
			return false;
		}
    }

    public override bool CanExecute()
    {
        if (this.waitCounter > 0)
        {
            this.waitCounter--;
            return false;
        }
        if (!this.theEntity.Spawned
            || !this.theEntity.IsAttackValid())
        {
			Debug.Log("CanExecute: Not spawned or !IsAttackValid");
            return false;
        }
        else
        {
            this.entityTarget = this.theEntity.GetAttackTarget();
            if (this.entityTarget == null) {
                return false;
            }
            float distanceSq = this.entityTarget.GetDistanceSq(this.theEntity);
            
			if(this.IsBelow(this.theEntity.GetPosition().z,
                                this.entityTarget.GetPosition().z,
                                distanceSq))
			{
				Debug.Log("CanExecute: "+this.entityTarget);
				return true;
			}
			else
			{
				return false;
			}
        }
    }
    
    public override void Start()
    {
		Debug.Log("Start");
        this.attackPeriod = this.attackPeriodMax;
    }

    public override bool Continue()
    {
        if (this.entityTarget == null
            || !this.entityTarget.IsAlive()
            || this.attackPeriod <= 0)
        {
			Debug.Log("Continue: abort");
            return false;
        }

        return true;
    }

    public override void Update()
    {
        this.attackPeriod--;
        if((float)this.attackPeriod > (float)this.attackPeriodMax * 0.5f)
        {
			Debug.Log("Update");
            //if(this.theEntity.IsInFrontOfMe(this.entityTarget.getHeadPosition()))
            //{
                this.theEntity.SetLookPosition(this.entityTarget.getHeadPosition());
            //}
            if(this.itemActionType == 0)
            {
                this.theEntity.Attack(this.attackPeriod == 0);
            }
            else
            {
                this.theEntity.Use(this.attackPeriod == 0);
            }
        }
    }

    public override void Reset()
    {
		Debug.Log("Reset");
        //this.entityTarget = null;
        this.attackPeriod = 0;
        this.waitCounter = 30 + UnityEngine.Random.Range(20, 50);

        //if (this.itemActionType == 0)
        //{
        //    this.theEntity.Attack(true);
        //}
        //else
        //{
        //    this.theEntity.Use(true);
        //}
    }
}
