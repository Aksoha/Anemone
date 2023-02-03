#pragma once
#include <memory>
#include <vector>
#include <numbers>

using vector = std::vector<double>;


#include "MatchingParameter.h"
#include "Topology.h"


template <class T>
concept IsTopology = std::is_base_of_v<Topology, T>;

template <IsTopology TTopology>
class Matching
{
protected:
	const vector _frequency;
	const vector _temperature;
	const vector _turnRatio;

	const double _voltageLimit;
	const double _currentLimit;
	const double _expectedPower;

	std::shared_ptr<TTopology> _topology;

	const double _nominalResistance;

	Matching(const MatchingParameter& data, std::shared_ptr<TTopology> topology);
};

template <IsTopology TTopology>
Matching<TTopology>::Matching(const MatchingParameter& data,  std::shared_ptr<TTopology> topology)
	: _frequency(static_cast<vector>(data.Frequency)),
	  _temperature(static_cast<vector>(data.Temperature)),
	  _turnRatio(static_cast<vector>(data.TurnRatio)),
	  _voltageLimit(data.VoltageLimit), _currentLimit(data.CurrentLimit),
	  _expectedPower(data.ExpectedPower),
	  _topology(topology),
	  _nominalResistance(_voltageLimit * _voltageLimit / _expectedPower * 8 / (std::numbers::pi * std::numbers::pi))
{
}
