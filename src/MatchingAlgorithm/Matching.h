#pragma once
#include <vector>
#include <numbers>

using vector = std::vector<double>;


#include "MatchingParameter.h"
#include "Topology.h"


template <class T>
concept IsTopology = std::is_base_of_v<Topology, T>;

template <class T>
concept IsParameter = std::is_base_of_v<MatchingParameter, T>;

template <IsParameter TParameter, IsTopology TTopology>
class Matching
{
protected:
	const vector _frequency;
	const vector _temperature;
	const vector _turnRatio;

	double _voltageLimit;
	double _currentLimit;
	double _expectedPower;

	const TTopology _topology;

	const double _nominalResistance;

	Matching(TParameter& data, TTopology& topology);
};

template <IsParameter TParameter, IsTopology TTopology>
Matching<TParameter, TTopology>::Matching(TParameter& data, TTopology& topology)
	: _frequency(static_cast<vector>(data.Frequency)),
	  _temperature(static_cast<vector>(data.Temperature)),
	  _turnRatio(static_cast<vector>(data.TurnRatio)),
	  _voltageLimit(data.VoltageLimit), _currentLimit(data.CurrentLimit),
	  _expectedPower(data.ExpectedPower),
	  _topology(topology),
	  _nominalResistance(_voltageLimit * _voltageLimit / _expectedPower * 8 / (std::numbers::pi * std::numbers::pi))
{
}
