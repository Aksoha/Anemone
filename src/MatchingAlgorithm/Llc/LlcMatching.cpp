#include "LlcMatching.h"

#include <algorithm>
#include <iterator>

#include "../SolutionNotFound.h"

LlcMatching::LlcMatching(const LlcMatchingParameter& data, std::shared_ptr<LlcTopology> topology)
	: Matching(data, topology),
	_inductance(static_cast<Vector>(data.Inductance)),
	_capacitance(static_cast<Vector>(data.Capacitance))
{
}

// TODO: delete after testing, also change wrapper signature
void LlcMatching::Match()
{
	Initialize();
	auto fLs = *std::ranges::find_if(_inductance.begin(), _inductance.end(), [this](const double x)
	{
		return x > _inductanceRange.Min;
	});

	
}

void LlcMatching::Initialize()
{
	unsigned long long index = 0;
	_topology->Capacitance = _capacitance[index];
	index++;
	auto inductanceRange = GetInductanceRange();

	while (inductanceRange.Min >= inductanceRange.Max)
	{
		if (index >= _capacitance.size())
			throw SolutionNotFound("no resonance for given range of frequency and capacitance");

		_topology->Capacitance = _capacitance[index];
		index++;
		inductanceRange = GetInductanceRange();
	}


	if (const auto it =
		std::ranges::find_if(_inductance.begin(), _inductance.end(), [&inductanceRange](const double x)
			{
				return x > inductanceRange.Min;
			}); it == _inductance.end())
		throw SolutionNotFound("solution requires a lower value of serial inductance then specified");
			_inductanceRange = inductanceRange;
}

void LlcMatching::ReduceData()
{
}

void LlcMatching::RestoreData()
{
}


InductanceRange LlcMatching::GetInductanceRange()
{
	auto inductanceMin = DBL_MIN;
	auto inductanceMax = DBL_MAX;

	for (const auto temperature : _temperature)
	{
		auto pairs = UpperResonanceRange(temperature);
		if (pairs->empty())
			return {};

		const auto reactanceMin = std::ranges::min_element(pairs->begin(), pairs->end(), [](auto lhs, auto rhs)
			{
				return lhs.Reactance < rhs.Reactance;
			});
		const auto reactanceMax = std::ranges::max_element(pairs->begin(), pairs->end(), [](auto lhs, auto rhs)
			{
				return lhs.Reactance < rhs.Reactance;
			});

		const auto lmax = -reactanceMin->Reactance / (2 * std::numbers::pi * reactanceMin->Frequency);
		const auto lmin = -reactanceMax->Reactance / (2 * std::numbers::pi * reactanceMax->Frequency);

		if (lmax < inductanceMax)
			inductanceMax = lmax;
		if (lmin > inductanceMin)
			inductanceMin = lmin;
	}

	return InductanceRange{ inductanceMin, inductanceMax };
}

std::unique_ptr<std::vector<FrequencyReactancePair>> LlcMatching::CapacitiveRange(const double temperature) const
{
	auto result = new std::vector<FrequencyReactancePair>();


	for (const auto frequency : _frequency)
	{
		if (const auto parallelReactance = _topology->ParallelReactance(frequency, temperature); parallelReactance < 0)
			result->push_back(FrequencyReactancePair{ frequency, parallelReactance });
	}

	return std::unique_ptr<std::vector<FrequencyReactancePair>>(result);
}

std::unique_ptr<std::vector<FrequencyReactancePair>> LlcMatching::UpperResonanceRange(const double temperature) const
{
	auto c = CapacitiveRange(temperature);
	auto it = std::ranges::min_element(c->begin(), c->end(), [](auto lhs, auto rhs)
		{
			return lhs.Reactance < rhs.Reactance;
		});

	// TODO: check if it will delete value pointed by it
	if (it != c->begin())
		--it;
	c->erase(c->begin(), it);
	return std::move(c);
}
