using System;
using UnityEngine;

public class EAITargetIsBelow : EAIBase
{
    private EntityAlive entityTarget;
    private DateTime lastAttemptTime;
    private double waitTimeSeconds;
    private int itemActionType;
    private int attackPeriod;
    private int attackPeriodMax;
	private float minYDistanceBelow;
	private float minimumDistance;

    public EAITargetIsBelow()
    {
        this.lastAttemptTime = System.DateTime.UtcNow;
        this.waitTimeSeconds = 7f;
        this.attackPeriodMax = 20;
		this.minYDistanceBelow = -2f;
		this.minimumDistance = 64f; // radius^2
    }

	private bool IsInCircle(float xEntity, float zEntity, float xTarget, float zTarget)
	{ 
		float xDiff = xTarget - xEntity;
		float zDiff = zTarget - zEntity;
		float distanceSq = (xDiff*xDiff + zDiff*zDiff);
		return distanceSq < this.minimumDistance;
	}
	
    private bool IsBelow(float yEntity, float yTarget)
    {
        float yDiff = yTarget - yEntity;
        return yDiff < this.minYDistanceBelow;
    }
	
	private bool IsInRange(Vector3 entityVector, Vector3 targetVector)
	{
		return this.IsBelow(entityVector.y, targetVector.y)
		&& this.IsInCircle(entityVector.x, entityVector.z, targetVector.x, targetVector.z);
	}

    private bool CanAttack()
    {
        if (!this.theEntity.Spawned)
        {
            return false;
        }
        else
        {
            this.entityTarget = this.theEntity.GetAttackTarget();
            return this.entityTarget != null
				&& this.entityTarget.IsAlive()
				&& this.IsInRange(this.theEntity.GetPosition(), this.entityTarget.GetPosition());
        }
    }

    public override void SetPar1(string _itemActionType)
    {
        itemActionType = int.Parse(_itemActionType);
    }

    public override void SetPar2(string _attackPeriodMax)
    {
        this.attackPeriodMax = int.Parse(_attackPeriodMax) * 20;
    }

    public override bool CanExecute()
    {
        DateTime currentTime = System.DateTime.UtcNow;
		double elapsedSeconds = (currentTime - this.lastAttemptTime).TotalSeconds;
        if ( elapsedSeconds < this.waitTimeSeconds)
        {
            return false;
        }
        this.lastAttemptTime = currentTime;
        return CanAttack();
    }
    
    public override void Start()
    {
        this.attackPeriod = this.attackPeriodMax;
    }

    public override bool Continue()
    {
        return CanAttack() && this.attackPeriod > 0;
    }

    public override void Update()
    {
        this.attackPeriod--;
        if((float)this.attackPeriod > (float)this.attackPeriodMax * 0.5f)
        {
            if(this.theEntity.IsInFrontOfMe(this.entityTarget.getHeadPosition()))
            {
                this.theEntity.SetLookPosition(this.entityTarget.getHeadPosition());
            }
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
        this.entityTarget = null;
        this.attackPeriod = 0;
        this.waitTimeSeconds = 6f + (float)UnityEngine.Random.Range(1, 4);

        if (this.itemActionType == 0)
        {
            this.theEntity.Attack(true);
        }
        else
        {
            this.theEntity.Use(true);
        }
    }
}
