#pragma once


class Topology
{
public:
	[[nodiscard]] virtual double Resistance(double frequency, double temperature) const = 0;
	[[nodiscard]] virtual double Reactance(double frequency, double temperature) const = 0;
	[[nodiscard]] virtual double Impedance(double frequency, double temperature) const = 0;

protected:
	Topology() = default;
};
