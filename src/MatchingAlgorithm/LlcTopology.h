#pragma once
#include "HeatingSystem.h"
#include "Topology.h"

class LlcTopology final : public Topology
{
public:
	double Inductance;
	double Capacitance;
	LlcTopology(HeatingSystem heatingSystem, double inductance, double capacitance);
	
	[[nodiscard]] double Resistance(double frequency, double temperature) const override;
	[[nodiscard]] double Reactance(double frequency, double temperature) const override;
	[[nodiscard]] std::complex<double> Impedance(double frequency, double temperature) const override;
	[[nodiscard]] double ParallelReactance(double frequency, double temperature) const;

private:
	HeatingSystem _heatingSystem;
};
