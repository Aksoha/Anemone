#include "SweepParameter.h"

#include <algorithm>
#include <iterator>
#include <stdexcept>

SweepParameter::operator std::vector<double>() const
{
	if (StepSize == 0.0)
		throw std::out_of_range("step size can't be equal to 0");

	if (fabs(Max - Min) == 0.0)
		throw std::out_of_range("range span can't be equal to 0");

	auto start = Min;
	auto end = Max;
	auto step = StepSize;
	std::vector<double> result;


	if (start > end)
	{
		const auto temp = start;
		start = end;
		end = temp;
		
	}
	std::generate_n(std::back_inserter(result), static_cast<size_t>((end - start) / step), [&start, step]()
	{
		const auto ret = start;
		start += step;
		return ret;
	});

	
	return result;
}
