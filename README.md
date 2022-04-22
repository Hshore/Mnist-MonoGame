# Mnist-MonoGame
A Machine Learning project. This project was to teach myself more about machine learning and neural networks. I decided not to use tensorflow or other machine learning libraries so I would get a better understanding of how the network actually works.

This network is built from scratch and uses supervised learning with gradient descent to identify handwritten digits in the MNIST dataset.

The netowrk can easily be modified to be a AutoEncoder or use different datasets for classification.

Recently I have changed the computation from CPU to GPU for a massive speedup. I used OpenCL to achieve this. 

NeuralNet.cs  contains old code for the NeuralNetwork that runs on the cpu only and is obsolete.

OpenCL.cs and KernalBuilder.cs contain the newer OpenCL implmentation.
