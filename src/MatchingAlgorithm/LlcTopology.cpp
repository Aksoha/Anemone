#include "LlcTopology.h"

#include <numbers>

double AngularFrequency(const double frequency)
{
	return 2 * std::numbers::pi * frequency;
}

LlcTopology::LlcTopology(HeatingSystem heatingSystem, const double inductance, const double capacitance)
	: Inductance(inductance), Capacitance(capacitance), _heatingSystem(std::move(heatingSystem))
{
	
}

double LlcTopology::Resistance(const double frequency, const double temperature) const
{
	const auto w = AngularFrequency(frequency);

	const auto resistance = _heatingSystem.Resistance(frequency, temperature);
	const auto inductance = _heatingSystem.Inductance(frequency, temperature);

	const auto req = resistance /
		(std::pow(1 - w * w * inductance * Capacitance, 2) +
			std::pow(w * Capacitance  * resistance, 2));

return req;
}

double LlcTopology::ParallelReactance(const double frequency, const double temperature) const
{
	const auto w = AngularFrequency(frequency);

	const auto resistance = _heatingSystem.Resistance(frequency, temperature);
	const auto inductance = _heatingSystem.Inductance(frequency, temperature);

	auto const xp = (w * inductance * (1 - w * w * inductance * Capacitance) - w * Capacitance * resistance * resistance) /
		(std::pow(1 * w * w * inductance * Capacitance, 2) + std::pow(w * Capacitance * resistance, 2));

	return xp;
}


double LlcTopology::Reactance(const double frequency, const double temperature) const
{
	const auto w = AngularFrequency(frequency);
	const auto x = w * Inductance * ParallelReactance(frequency, temperature);

	return x;
}


std::complex<double> LlcTopology::Impedance(const double frequency, const double temperature) const
{
	const auto resistance = Resistance(frequency, temperature);
	const auto reactance = Reactance(frequency, temperature);
	
	auto const z = std::complex(resistance, reactance);

	return z;
	
}

