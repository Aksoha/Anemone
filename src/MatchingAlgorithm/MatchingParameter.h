#pragma once
#include "SweepParameter.h"

struct MatchingParameter
{
	SweepParameter Frequency;
	SweepParameter Temperature;
	SweepParameter TurnRatio;
	double VoltageLimit;
	double CurrentLimit;
	double ExpectedPower;
};
