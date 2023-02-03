#pragma once
#include <memory>

#include "../HeatingSystem.h"
#include "../Topology.h"

class LlcTopology final : public Topology
{
public:
	double Inductance;
	double Capacitance;
	LlcTopology(std::shared_ptr<HeatingSystem> heatingSystem, double inductance, double capacitance);

	[[nodiscard]] double Resistance(double frequency, double temperature) const override;
	[[nodiscard]] double Reactance(double frequency, double temperature) const override;
	[[nodiscard]] double Impedance(double frequency, double temperature) const override;
	[[nodiscard]] double ParallelReactance(double frequency, double temperature) const;

private:
	std::shared_ptr<HeatingSystem> _heatingSystem;
	
};
