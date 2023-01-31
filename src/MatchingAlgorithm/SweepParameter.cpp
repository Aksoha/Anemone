#include "SweepParameter.h"

#include <algorithm>
#include <iterator>

SweepParameter::operator std::vector<double>() const
{

	auto start = Min;
	const auto end = Max;
	auto step = StepSize;
	std::vector<double> result;

	std::generate_n(std::back_inserter(result), static_cast<size_t>((end - start) / step), [&start, step]() { const auto ret = start; start += step; return ret; });

	return result;
}
