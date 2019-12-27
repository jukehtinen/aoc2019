package main

import (
	"fmt"
	"io/ioutil"
	"strconv"
	"strings"
)

func main() {
	data, _ := ioutil.ReadFile("input.txt")
	s := string(data)
	input := make([]int, len(s))
	for i := 0; i < len(input); i++ {
		input[i], _ = strconv.Atoi(string(s[i]))
	}

	// Part 1
	basePattern := []int{0, 1, 0, -1}
	output := make([]int, len(input))
	for p := 0; p < 100; p++ {

		for i := range input {

			result := 0
			for j := range input {

				patternIndex := (j + 1) / (i + 1) % 4
				result += input[j] * basePattern[patternIndex]
			}

			// abs
			if result < 0 {
				result = -result
			}

			output[i] = result % 10
		}
		input = output
	}
	print(strings.Join(strings.Fields(fmt.Sprint(output[0:8])), ""))
}
