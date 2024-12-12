#include <Arduino.h>

#define MAX_BLOCKS 1000  // Maximum number of blocks to store in SRAM

byte blocks[MAX_BLOCKS];  // Array to store blocks (in SRAM)
int numBlocks = 0;

void parseDiskMapFromSerial() {
  while (Serial.available() > 0 && numBlocks < MAX_BLOCKS) {
    char c = Serial.read();

    if (c >= '0' && c <= '9') {
      byte fileLength = c - '0';  // Convert char to byte
      if (Serial.available() > 0) {
        c = Serial.read();
        byte freeLength = c - '0';  // Convert char to byte

        // Add file blocks
        for (int j = 0; j < fileLength; j++) {
          blocks[numBlocks++] = 1;  // Assume all files are ID 1 for simplicity
        }

        // Add free space blocks
        for (int j = 0; j < freeLength; j++) {
          blocks[numBlocks++] = 255;  // 255 represents free space
        }
      }
    }
  }
}

void compactDisk(byte blocks[], int numBlocks) {
  // Process files from the end to the beginning
  for (int i = numBlocks - 1; i >= 0; i--) {
    if (blocks[i] != 255) {  // If it's a file block
      // Find the first free space (represented by 255)
      int j = 0;
      while (j < numBlocks && blocks[j] != 255) {
        j++;
      }

      // If there's free space available
      if (j < numBlocks) {
        blocks[j] = blocks[i];  // Move the file to the free space
        blocks[i] = 255;  // Mark the original position as free space
      }
    }
  }
}

int calculateChecksum(byte blocks[], int numBlocks) {
  int checksum = 0;
  for (int i = 0; i < numBlocks; i++) {
    if (blocks[i] != 255) {  // Skip free space blocks
      checksum += i * blocks[i];  // Position * File ID
    }
  }
  return checksum;
}

void setup() {
  Serial.begin(9600);  // Start serial communication

  // Wait for data from serial input
  Serial.println("Enter disk map (e.g., '233313'):");

  // Parse the disk map from serial input
  parseDiskMapFromSerial();

  // Compact the disk
  compactDisk(blocks, numBlocks);

  // Calculate and print the checksum
  int checksum = calculateChecksum(blocks, numBlocks);
  Serial.print("Resulting checksum: ");
  Serial.println(checksum);
}