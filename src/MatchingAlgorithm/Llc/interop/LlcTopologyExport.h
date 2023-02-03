#pragma once
#pragma once
#include "../LlcTopology.h"
#include "../../interop/export.h"

DLL_EXPORT LlcTopology* LlcTopology_Create(HeatingSystem* heatingSystem, double inductance, double capacitance);
DLL_EXPORT void LlcTopology_Dispose(const LlcTopology* llc);
DLL_EXPORT double LlcTopology_Resistance(const LlcTopology* llc, double frequency, double temperature);
DLL_EXPORT double LlcTopology_Reactance(const LlcTopology* llc, double frequency, double temperature);
DLL_EXPORT double LlcTopology_Impedance(const LlcTopology* llc, double frequency, double temperature);
DLL_EXPORT double LlcTopology_ParallelReactance(const LlcTopology* llc, double frequency, double temperature);
DLL_EXPORT void LlcTopology_SetInductance(LlcTopology* llc, double inductance);
DLL_EXPORT void LlcTopology_SetCapacitance(LlcTopology* llc, double capacitance);
