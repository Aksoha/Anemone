#pragma once
#include "../MatchingParameter.h"

struct LlcMatchingParameter : MatchingParameter
{
	SweepParameter Inductance;
	SweepParameter Capacitance;
};
