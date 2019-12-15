#include <charconv>
#include <fstream>
#include <iostream>
#include <cmath>
#include <regex>
#include <string>
#include <vector>

struct Moon
{
	int x, y, z;
	int vx, vy, vz;
	int getEnergy() { return (std::abs(x) + std::abs(y) + std::abs(z)) * (std::abs(vx) + std::abs(vy) + std::abs(vz)); };
};

int main()
{
	std::vector<char> buf(123);
	std::vector<Moon> moons;
	std::vector<Moon> moonInit;

	std::ifstream file("C:\\Dev\\aoc2019\\day12\\input.txt");
	std::string line;
	std::regex rx("x=(.*), y=(.*), z=(.*)>");
	while (std::getline(file, line))
	{
		std::smatch match;
		std::regex_search(line, match, rx);

		moons.push_back({ std::atoi(match[1].str().c_str()), std::atoi(match[2].str().c_str()), std::atoi(match[3].str().c_str()) });
		moonInit.push_back({ std::atoi(match[1].str().c_str()), std::atoi(match[2].str().c_str()), std::atoi(match[3].str().c_str()) });
	}

	// Part 1
	for (int i = 0; i < 1000; i++)
	{
		// apply gravity
		for (auto& m : moons)
		{
			for (const auto& pair : moons)
			{
				if (&pair == &m) continue;

				if (m.x > pair.x) m.vx--;
				if (m.x < pair.x) m.vx++;
				if (m.y > pair.y) m.vy--;
				if (m.y < pair.y) m.vy++;
				if (m.z > pair.z) m.vz--;
				if (m.z < pair.z) m.vz++;
			}
		}

		// apply velocity
		for (auto& m : moons)
		{
			m.x += m.vx;
			m.y += m.vy;
			m.z += m.vz;
		}
	}

	int totalEnergy = 0;
	for (auto& m : moons)
	{
		totalEnergy += m.getEnergy();
	}

	std::cout << totalEnergy << "\n";
	
	// Part 2
	int periodx = 1000;
	while (true)
	{
		// apply gravity
		for (auto& m : moons)
		{
			for (const auto& pair : moons)
			{
				if (&pair == &m) continue;

				if (m.x > pair.x) m.vx--;
				if (m.x < pair.x) m.vx++;
				if (m.y > pair.y) m.vy--;
				if (m.y < pair.y) m.vy++;
				if (m.z > pair.z) m.vz--;
				if (m.z < pair.z) m.vz++;
			}
		}

		// apply velocity
		for (auto& m : moons)
		{
			m.x += m.vx;
			m.y += m.vy;
			m.z += m.vz;
		}

		bool ok = true;
		for (int i = 0; i < 4; i++)
		{
			if (moons[i].x != moonInit[i].x || moons[i].vx != moonInit[i].vx)
				ok = false;
		}
		if (ok)
			break;

		periodx++;
	}

	std::cout << "x period: " << periodx << "\n";
	// Todo

	return 0;
}
