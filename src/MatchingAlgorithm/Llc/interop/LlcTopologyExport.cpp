#include "LlcTopologyExport.h"

LlcTopology* LlcTopology_Create(HeatingSystem* heatingSystem, const double inductance, const double capacitance)
{
	const auto ptr = std::shared_ptr<HeatingSystem>(heatingSystem);
	return new LlcTopology(ptr, inductance, capacitance);
}

void LlcTopology_Dispose(const LlcTopology* llc)
{
	delete llc;
}

double LlcTopology_Resistance(const LlcTopology* llc, const double frequency, const double temperature)
{
	return llc->Resistance(frequency, temperature);
}

double LlcTopology_Reactance(const LlcTopology* llc, const double frequency, const double temperature)
{
	return llc->Reactance(frequency, temperature);
}

double LlcTopology_Impedance(const LlcTopology* llc, const double frequency, const double temperature)
{
	return llc->Impedance(frequency, temperature);
}

double LlcTopology_ParallelReactance(const LlcTopology* llc, const double frequency, const double temperature)
{
	return llc->ParallelReactance(frequency, temperature);
}

void LlcTopology_SetInductance(LlcTopology* llc, const double inductance)
{
	llc->Inductance = inductance;
}

void LlcTopology_SetCapacitance(LlcTopology* llc, const double capacitance)
{
	llc->Capacitance = capacitance;
}
