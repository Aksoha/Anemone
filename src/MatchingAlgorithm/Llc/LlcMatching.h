#pragma once
#include "FrequencyReactancePair.h"
#include "InductanceRange.h"
#include "LlcMatchingParameter.h"
#include "LlcTopology.h"
#include "../Matching.h"

using Vector = std::vector<double>;

class LlcMatching : public Matching<LlcTopology>
{

protected:
	const Vector _inductance;
	const Vector _capacitance;
	InductanceRange _inductanceRange{};
public:
	LlcMatching(const LlcMatchingParameter& data, std::shared_ptr<LlcTopology> topology);
	void Match();

protected:
	void Initialize();
	void SetOutputParameters();
	void ReduceData();
	void RestoreData();

private:
	InductanceRange GetInductanceRange();
	[[nodiscard]] std::unique_ptr<std::vector<FrequencyReactancePair>> CapacitiveRange(double temperature) const;
	[[nodiscard]] std::unique_ptr<std::vector<FrequencyReactancePair>> UpperResonanceRange(double temperature) const;
	void TurnRatioSetting();
	
};
