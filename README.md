# QuadTreeImageCompression
Simple image compression with the quadtree data structure.

---

## Usage
```bash
.\QuadTreeImageCompression <input-image-path> <output-imagepath>
```
There are some optional parameters:
- Radius of color sphere in which colors are considered "same"
```
-r <float> 
--radius <float>
``` 
Default: ```100.0```
- Maximum quadtree depth (maximum amount of subdivisions of the root quadtree)
```
-d <int>
--max-depth <int>
```
Default: ```0``` (biggest possible grid maximum depth is chosen)
- Show quadtree grid
```
-g
--show-grid
```
Default: ```false```
- Quadtree grid color
```
-c <hex-color>
--grid-color <hex-color>
```
Default: ```#ffffff```(White)
## Description
#### "Same" colors
Let's define "sameness" of colors
Colors are considered "same" if the linear distance between them is less than or equal to ```--radius```


!["Sameness" illustration](https://raw.githubusercontent.com/Kentoso/QuadTreeImageCompression/master/img/sameness.png)
**1** has no "same" colors (apart from itself)
**2** is "same" as **2**, **3** and **4**
**3** is "same" as **2**, **3**
**4** is "same" as **2**, **4**

> **Note**
"Sameness" is symmetric, reflexive, and not transitive

#### Procedure
Quad tree iterates through all pixels in its region, then:
1. _if_ all colors are considered "same" - calculate average color of the region and set all pixels in the region to this color
2. _if_ at least one color is considered "not same", then we subdivide quadtree into four children and repeat this procedure for each of them 
> **Note** 
if a quadtree cannot be subdivided, then its colors are considered "same"

# Examples
Source image
![Pug source](https://raw.githubusercontent.com/Kentoso/QuadTreeImageCompression/master/img/pugsource.jpg)

```
.\QuadTreeImageCompression pugsource.jpg output1.png 
```
![Output1](https://raw.githubusercontent.com/Kentoso/QuadTreeImageCompression/master/img/output1.png)

```
.\QuadTreeImageCompression pugsource.jpg output2.png -r 25
```
![Output2](https://raw.githubusercontent.com/Kentoso/QuadTreeImageCompression/master/img/output2.png)

```
.\QuadTreeImageCompression pugsource.jpg output3.png -r 25 -d 5
```
![Output3](https://raw.githubusercontent.com/Kentoso/QuadTreeImageCompression/master/img/output3.png)

```
.\QuadTreeImageCompression pugsource.jpg output4.png -r 200 -d 5 -g
```
![Output4](https://raw.githubusercontent.com/Kentoso/QuadTreeImageCompression/master/img/output4.png)

```
.\QuadTreeImageCompression pugsource.jpg output5.png -r 200 -d 5 -g -c 00000000
```
![Output5](https://raw.githubusercontent.com/Kentoso/QuadTreeImageCompression/master/img/output5.png)