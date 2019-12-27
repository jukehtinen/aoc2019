import java.util.HashSet;

public class day24 {

  public static int getBugs(int[][] map, int x, int y) {
    int bugs = 0;
    if (x > 0 && map[x - 1][y] == 1) bugs++;
    if (x < 4 && map[x + 1][y] == 1) bugs++;
    if (y > 0 && map[x][y - 1] == 1) bugs++;
    if (y < 4 && map[x][y + 1] == 1) bugs++;
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

    HashSet<String> seen = new HashSet<String>();
    seen.add(getMapString(map));
    while (true) {
      int[][] next = new int[5][5];
      for (int y = 0; y < 5; y++) {
        for (int x = 0; x < 5; x++) {
          int adjacentBugs = getBugs(map, x, y);
          // A bug dies (becoming an empty space) unless there is exactly one bug adjacent o it.
          if (map[x][y] == 1) {
            next[x][y] = adjacentBugs == 1 ? 1 : 0;
          }
          // An empty space becomes infested with a bug if exactly one or two bugs are adjacent to it.
          if (map[x][y] == 0) {
            next[x][y] = (adjacentBugs == 1 || adjacentBugs == 2) ? 1 : 0;
          }
        }
      }
      if (!seen.add(getMapString(next))) {
        char[] s = getMapString(next).toCharArray();
        int result = 0;
        for (int i = 0; i < s.length; i++) {
          if (s[i] == '#')
            result += (int) Math.pow(2, i);
        }
        System.out.println(result);
        break;
      }
      map = next;
    }
  }
}