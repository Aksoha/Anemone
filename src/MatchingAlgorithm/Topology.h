#pragma once
#include <complex>


class Topology
{
public:
	[[nodiscard]] virtual double  Resistance(double frequency, double temperature) const = 0;
	[[nodiscard]] virtual double Reactance(double frequency, double temperature) const = 0;
	[[nodiscard]] virtual std::complex<double> Impedance(double frequency, double temperature) const = 0;

	virtual ~Topology() = 0;
};
