#pragma once

#include <vector>

struct SweepParameter
{
public:
	double Min;
	double Max;
	double StepSize;

	explicit operator std::vector<double>() const;
};
