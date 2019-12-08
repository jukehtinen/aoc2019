package main

import (
	"fmt"
	"io/ioutil"
	"math"
	"strconv"
)

func count(pixs []int, val int) int {
	count := 0
	for i := 0; i < len(pixs); i++ {
		if pixs[i] == val {
			count++
		}
	}
	return count
}

func main() {
	data, _ := ioutil.ReadFile("input.txt")
	s := string(data)
	pixels := make([]int, len(s))
	pixelCount := 25 * 6
	layerCount := len(pixels) / pixelCount
	for i := 0; i < len(pixels); i++ {
		pixels[i], _ = strconv.Atoi(string(s[i]))
	}

	// Part 1
	fewest := math.MaxUint32
	fewestLayer := 0
	for l := 0; l < layerCount; l++ {
		pixs := pixels[l*pixelCount : (l*pixelCount)+pixelCount]
		zeroes := count(pixs, 0)
		if zeroes < fewest {
			fewest = zeroes
			fewestLayer = l
		}
	}
	pixs := pixels[fewestLayer*pixelCount : (fewestLayer*pixelCount)+pixelCount]
	fmt.Println(count(pixs, 1) * count(pixs, 2))

	// Part 2
	image := make([]int, pixelCount)
	for i := 0; i < pixelCount; i++ {
		for l := 0; l < layerCount; l++ {
			if pixels[(pixelCount*l)+i] != 2 {
				image[i] = pixels[(pixelCount*l)+i]
				break
			}
		}
	}
	for i := 0; i < pixelCount; i++ {
		if i%25 == 0 {
			fmt.Println()
		}
		if image[i] == 0 {
			fmt.Print(".")
		} else {
			fmt.Print("#")
		}
	}
}
