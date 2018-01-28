import ij.*;
import ij.process.*;
import ij.plugin.filter.*;
import ij.gui.*;
import ij.io.*;

import java.io.*;
import java.util.*;

/**
 * FWT
 * Exercise 11.2
 * 
 * @author Christoph Stamm
 *
 */
public class FWT_ implements PlugInFilter {
	ImagePlus imp;

	public int setup(String arg, ImagePlus imp) {
		IJ.register(FWT_.class);
		this.imp = imp;
		return DOES_8G + NO_CHANGES;
	}

	public void run(ImageProcessor ip) {
		final int w = ip.getWidth();
		final int h = ip.getHeight();
		
		int[][] data = ip.getIntArray(); // data is column major organized
		
		// forward FWT
		fwt(data);
		
		// inverse FWT
		ifwt(data);
		
		// produce output image
		ImageProcessor dst = new ByteProcessor(w, h);
		ImagePlus dstImg = imp.createImagePlus();
		dstImg.setProcessor("1D FWT encoded/decoded", dst);
		dst.setIntArray(data);
		dstImg.show();
		
		// forward 2D FWT
		fwt2D(data);
		
		// produce output image
		final int w2 = w/2;
		final int h2 = h/2;
		ImageProcessor dst1 = new ByteProcessor(w2, h2);
		ImagePlus dstImg1 = imp.createImagePlus();
		dstImg1.setProcessor("Lowpass of 2D FWT encoded", dst1);
		for(int u=0; u < w2; u++) {
			int[] col = data[2*u];
			for(int v=0; v < h2; v++) {
				dst1.set(u, v, clamp(col[2*v] >> 6)); // divide by 8*8
			}
		}
		dstImg1.show();

		// inverse 2D FWT
		ifwt2D(data);
		
		// produce output image
		ImageProcessor dst2 = new ByteProcessor(w, h);
		ImagePlus dstImg2 = imp.createImagePlus();
		dstImg2.setProcessor("2D FWT encoded/decoded", dst2);
		dst2.setIntArray(data);
		dstImg2.show();
	}
	
	/**
	 * 1D horizontal FWT with Daubechies 5/3 filter
	 * data is column major organized
	 * To keep integer precision don't apply factor 1/8 to resulting data 
	 * Resulting data has to interleave Low- and High-pass: L,H,L,H,...,L,H
	 * in-situ processing, don't allocate a new array
	 * @param ip
	 */
	private void fwt(int[][] data) {
		// TODO
	}
	
	/**
	 * 1D horizontal FWT with Daubechies 5/3 filter
	 * data is column major organized
	 * data has integer precision (factor 1/8 hasn't been applied)
	 * data has interleaved Low- and High-pass: L,H,L,H,...,L,H
	 * @param ip
	 */
	private void ifwt(int[][] data) {
		final int ld8 = 3; // ld of factor 8
		final int w = data.length;
		assert w%2 == 0;
		
		for (int u = 0; u < w; u++) {
			int[] col = data[u];
			final int h = col.length;
			assert h%2 == 0;
			
			int p0 =  col[0], p1 = col[1], p2 = col[2];
			// top border handling
			col[0] = (p0 - p1) >> ld8; 
			col[1] = (2*p0 + 5*p1 + 2*p2 - col[3]) >> (ld8 + 2); 
			p0 = p1; p1 = p2;
			// middle part
			for (int v = 2; v < h - 2; v += 2) {
				p2 = col[v+1];
				col[v] = (-p0 + 2*p1 - p2) >> (ld8 + 1); 
				col[v+1] = (-p0 + 2*p1 + 6*p2 + 2*col[v+2] - col[v+3]) >> (ld8 + 2); 
				p0 = p2;
				p1 = col[v+2];
			}
			// bottom border handling
			p2 = col[h-1];
			col[h-2] = (-p0 + 2*p1 - p2) >> (ld8 + 1); 
			col[h-1] = (-p0 + 2*p1 + 3*p2) >> (ld8 + 1); 
		}
	}
	
	/**
	 * in and out are column major organized
	 * 
	 */
	private void transpose(int[][] in, int[][] out) {
		// TODO
	}
	
	private void fwt2D(int[][] data) {
		// TODO
	}
	
	private void ifwt2D(int[][] data) {
		// TODO
	}
	
	private int clamp(int val) {
		if (val > 255) return 255;
		else if (val < 0) return 0;
		return val;
	}
}
