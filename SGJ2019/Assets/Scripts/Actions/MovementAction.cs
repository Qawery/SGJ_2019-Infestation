﻿using UnityEngine;


namespace SGJ2019
{
	public class MovementAction : Action
	{
		public override string Name => "Move";
		public override string Description => "Move to adjacent space" + "\n" +
												base.Description;
		public override int Cost => 1;


		public override bool IsActionPossible(CardSlot source, CardSlot target)
		{
			var actionPoints = source.Card.GetComponent<ActionPointsComponent>();
			if (actionPoints != null && actionPoints.CurrentActionPoints < Cost)
			{
				return false;
			}
			var rowManager = source.transform.parent.GetComponent<RowManager>();
			int selectedIndex = rowManager.GetIndexOfCard(source.Card);
			int otherIndex = rowManager.GetIndexOfCard(target.Card);
			return Mathf.Abs(selectedIndex - otherIndex) == 1;
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
				var rowManager = source.transform.parent.GetComponent<RowManager>();
				int sourceIndex = rowManager.GetIndexOfCard(source.Card);
				int targetIndex = rowManager.GetIndexOfCard(target.Card);
				if (sourceIndex < targetIndex)
				{
					LogManager.Instance.AddMessage(source.Card.CardName + " moved right");
					rowManager.MoveCardRight(source);
				}
				else if (sourceIndex > targetIndex)
				{
					LogManager.Instance.AddMessage(source.Card.CardName + " moved left");
					rowManager.MoveCardLeft(source);
				}
			}
		}
	}
}