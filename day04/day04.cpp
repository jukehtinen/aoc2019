#include <charconv>
#include <iostream>
#include <string>

int main()
{
	std::string input = "138241-674034";

	auto l1 = std::atoi(input.substr(0, input.find_first_of('-')).c_str());
	auto l2 = std::atoi(input.substr(input.find_first_of('-') + 1).c_str());

	static auto toDigit = [](const std::string& input, int index) -> int {
		auto val = 0;
		std::from_chars(input.data() + index, input.data() + index + 1, val);
		return val;
	};

	// Two adjacent digits are the same
	auto adjacentDigits = [](int value) -> bool {
		auto v = std::to_string(value);
		for (auto j = 0; j < v.size() - 1; j++)
		{
			if (toDigit(v, j) == toDigit(v, j + 1))
				return true;
		}
		return false;
	};

	// two adjacent matching digits are not part of a larger group of matching digits
	auto adjacentDigitsPart2 = [](int value) -> bool {
		auto v = std::to_string(value);
		auto group = toDigit(v, 0);
		auto groupSize = 1;
		for (auto j = 1; j < v.size(); j++)
		{
			if (toDigit(v, j) == group)
			{
				groupSize++;
			}
			else
			{
				if (groupSize == 2)
					return true;
				groupSize = 1;
			}
			group = toDigit(v, j);
		}
		return groupSize == 2;
	};

	// Going from left to right, the digits never decrease
	auto digitsIncrease = [](int value) -> bool {
		auto v = std::to_string(value);
		auto n = toDigit(v, 0);
		for (auto j = 1; j < v.size(); j++)
		{
			auto n2 = toDigit(v, j);
			if (n2 < n)
				return false;
			n = n2;
		}
		return true;
	};

	int count = 0;
	for (auto i = l1; i < l2; i++)
	{
		if (adjacentDigits(i) && digitsIncrease(i))
			count++;
	}
	std::cout << "Part 1: " << count << "\n";

	count = 0;
	for (auto i = l1; i < l2; i++)
	{
		if (adjacentDigits(i) && digitsIncrease(i) && adjacentDigitsPart2(i))
			count++;
	}
	std::cout << "Part 2: " << count << "\n";

	return 0;
}
