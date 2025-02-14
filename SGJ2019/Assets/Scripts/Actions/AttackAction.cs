﻿using UnityEngine;
using UnityEngine.Assertions;


namespace SGJ2019
{
	public class AttackAction : PlayerAction
	{
		public override string Name => "Attack";
		public override string Description => "Attack target in row" + "\n" + "Damage: " + Damage.ToString() + "  Range: " + MinRange.ToString() + (MinRange == MaxRange ? "" : "-" + MaxRange.ToString()) + "\n" +
												base.Description;
		public override int Cost => 1;
		public virtual int Damage => 1;
		public virtual int MinRange => 1;
		public virtual int MaxRange => 1;


		public AttackAction()
		{
			Assert.IsTrue(MinRange > 0);
			Assert.IsTrue(MaxRange > 0);
			Assert.IsTrue(MaxRange >= MinRange);
		}

		public override bool IsActionPossible(CardSlot source, CardSlot target)
		{
			var actionPoints = source.Card.GetComponent<ActionPointsComponent>();
			if (actionPoints != null && actionPoints.CurrentActionPoints < Cost)
			{
				Utilities.SpawnFloatingText("Insufficient action points!", Color.red, source.Card.transform);
				return false;
			}
			var targetHealthComponent = target.Card.GetComponent<HealthComponent>();
			if (targetHealthComponent == null)
			{
				Utilities.SpawnFloatingText("Invalid target!", Color.red, source.Card.transform);
				return false;
			}
			var rowManager = source.transform.parent.GetComponent<RowManager>();
			if (rowManager != target.transform.parent.GetComponent<RowManager>())
			{
				Utilities.SpawnFloatingText("Invalid row!", Color.red, source.Card.transform);
				return false;
			}
			if (IsTargetOutsideRange(rowManager, source, target))
			{
				Utilities.SpawnFloatingText("Outside range!", Color.red, source.Card.transform);
				return false;
			}
			return true;
		}

		public override void ExecuteAction(CardSlot source, CardSlot target)
		{
			if (IsActionPossible(source, target))
			{
				var actionPoints = source.Card.GetComponent<ActionPointsComponent>();
				if (actionPoints != null)
				{
					actionPoints.Spend(Cost);
				}
				var targetHealthComponent = target.Card.GetComponent<HealthComponent>();
				targetHealthComponent.Damage(Damage);
				Utilities.SpawnFloatingText(Name, Color.grey, source.Card.transform);
				LogManager.Instance.AddMessage(source.Card.CardName + " attacked " + target.Card.CardName + " with " + Name + " for " + Damage.ToString() + " damage");
				AudioManager.Instance.PlaySound(Name);
			}
		}

		private bool IsTargetOutsideRange(RowManager rowManager, CardSlot source, CardSlot target)
		{
			int selectedIndex = rowManager.GetIndexOfCard(source.Card);
			int otherIndex = rowManager.GetIndexOfCard(target.Card);
			int distance = Mathf.Abs(selectedIndex - otherIndex);
			if (distance >= MinRange && distance <= MaxRange)
			{
				return false;
			}
			return true;
		}
	}
}