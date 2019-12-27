import java.util.Collections;
import java.util.HashMap;
import java.util.Map;

public class day24part2 {

  public static int getBugsLower(int[][] map, int dir) {
    int bugs = 0;
    if (dir == 1) {
      for (int i = 0; i < 5; i++) bugs += map[4][i];
    } else if (dir == 0) {
      for (int i = 0; i < 5; i++) bugs += map[i][0];
    } else if (dir == 3) {
      for (int i = 0; i < 5; i++) bugs += map[0][i];
    } else if (dir == 2) {
      for (int i = 0; i < 5; i++) bugs += map[i][4];
    }
    return bugs;
  }

  public static int getBugsUpper(int[][] map, int dir) {
    if (dir == 3) {
      return map[1][2];
    }
    if (dir == 0) {
      return map[2][1];
    }
    if (dir == 1) {
      return map[3][2];
    }
    if (dir == 2) {
      return map[2][3];
    }
    return 0;
  }

  public static int getBugs(Map<Integer, int[][]> levels, int currentLevel, int x, int y) {
    int[][] map = levels.get(currentLevel);

    int bugs = 0;
    if (x - 1 == 2 && y == 2 && levels.containsKey(currentLevel - 1)) {
      bugs += getBugsLower(levels.get(currentLevel - 1), 1);
    } else if (x - 1 < 0 && levels.containsKey(currentLevel + 1)) {
      bugs += getBugsUpper(levels.get(currentLevel + 1), 3);
    } else if (x > 0 && map[x - 1][y] == 1)
      bugs++;

    if (x + 1 == 2 && y == 2 && levels.containsKey(currentLevel - 1)) {
      bugs += getBugsLower(levels.get(currentLevel - 1), 3);
    } else if (x + 1 > 4 && levels.containsKey(currentLevel + 1)) {
      bugs += getBugsUpper(levels.get(currentLevel + 1), 1);
    } else if (x < 4 && map[x + 1][y] == 1)
      bugs++;

    if (x == 2 && y - 1 == 2 && levels.containsKey(currentLevel - 1)) {
      bugs += getBugsLower(levels.get(currentLevel - 1), 0);
    } else if (y - 1 < 0 && levels.containsKey(currentLevel + 1)) {
      bugs += getBugsUpper(levels.get(currentLevel + 1), 2);
    } else if (y > 0 && map[x][y - 1] == 1)
      bugs++;

    if (x == 2 && y + 1 == 2 && levels.containsKey(currentLevel - 1)) {
      bugs += getBugsLower(levels.get(currentLevel - 1), 2);
    } else if (y + 1 > 4 && levels.containsKey(currentLevel + 1)) {
      bugs += getBugsUpper(levels.get(currentLevel + 1), 0);
    } else if (y < 4 && map[x][y + 1] == 1)
      bugs++;
    return bugs;
  }

  public static String getMapString(int[][] map) {
    String out = "";
    for (int y = 0; y < 5; y++) {
      for (int x = 0; x < 5; x++) {
        out += map[x][y] == 1 ? "#" : ".";
      }
    }
    return out;
  }

  public static void main(String[] args) {
    int[][] map = new int[5][5];
    char[] input = "##.##.#.####..##.#...###.".toCharArray();
    for (int i = 0; i < input.length; i++) {
      map[i % 5][i / 5] = input[i] == '#' ? 1 : 0;
    }

    Map<Integer, int[][]> levels = new HashMap<Integer, int[][]>();
    levels.put(0, map);

    for (int i = 0; i < 200; i++) {
      // Add new levels if there are bugs on current lower/upper ones
      int min = Collections.min(levels.keySet());
      if (getMapString(levels.get(min)).contains("#")) {
        levels.put(min - 1, new int[5][5]);
      }

      int max = Collections.max(levels.keySet());
      if (getMapString(levels.get(max)).contains("#")) {
        levels.put(max + 1, new int[5][5]);
      }

      Map<Integer, int[][]> newLevels = new HashMap<Integer, int[][]>();
      for (Map.Entry<Integer, int[][]> level : levels.entrySet()) {
        int[][] next = new int[5][5];
        newLevels.put(level.getKey(), next);
        for (int y = 0; y < 5; y++) {
          for (int x = 0; x < 5; x++) {
            if (x == 2 && y == 2) continue;
            int adjacentBugs = getBugs(levels, level.getKey(), x, y);
            // A bug dies (becoming an empty space) unless there is exactly one bug adjacent to it.
            if (level.getValue()[x][y] == 1) {
              next[x][y] = adjacentBugs == 1 ? 1 : 0;
            }
            // An empty space becomes infested with a bug if exactly one or two bugs are adjacent to it.
            if (level.getValue()[x][y] == 0) {
              next[x][y] = (adjacentBugs == 1 || adjacentBugs == 2) ? 1 : 0;
            }
          }
        }
      }

      levels = newLevels;
    }

    int total = 0;
    for (Map.Entry<Integer, int[][]> level : levels.entrySet()) {
      total += getMapString(level.getValue()).chars().filter(ch -> ch == '#').count();
    }
    System.out.println(total);
  }
}