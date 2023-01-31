#pragma once
#include <complex>

struct MatchingResult
{
	double Resistance{};
	double Reactance{};
	std::complex<double> Impedance;
	double Frequency{};
	double Voltage{};
	double Current{};
	double Power{};
	double TurnRatio{};
};
