Feature: Init3Node

@mytag
Scenario: Cluster with 2 node and A ,B
	Given Nodes of the cluster are
	| NodeName |
	| A    |  
	| B	   |  
	And messages in 'A' are
	| SequenceId | Node | Topic | Data |
	| 1          | A	|       |      |
	| 2          | A	  |       |      |
	When 'B' send UpdateRequest to 'A' 
	Then the response should be 
	| SequenceId | Node | Topic | Data |
	| 1          | A  |       |      |
	| 2          | A  |       |      |


@mytag
Scenario: Cluster with 3 node and C ask update to A
	Given Nodes of the cluster are
	| NodeName |
	| A    |  
	| B	   |  
	| C  |  
	And messages in 'C' are
	| SequenceId | Node | Topic | Data |
	| 1          | A	|       |      |
	| 1          | B	  |       |      |
	And messages in 'A' are
	| SequenceId | Node | Topic | Data |
	| 1          | A  |       |      |
	| 2          | A  |       |      |
	| 3          | A  |       |      |
	| 1          | B	  |       |      |
	And messages in 'B' are
	| SequenceId | Node | Topic | Data |
	| 1          | A  |       |      |
	| 1          | B	  |       |      |
	| 2          | B	  |       |      |
	When 'C' send UpdateRequest to 'A' 
	Then the response should be 
	| SequenceId | Node | Topic | Data |
	| 2          | A  |       |      |
	| 3          | A  |       |      |

Scenario: Cluster with 3 node and C ask update to B
	Given Nodes of the cluster are
	| NodeName |
	| A    |  
	| B	   |  
	| C  |  
	And messages in 'C' are
	| SequenceId | Node | Topic | Data |
	| 1          | A	|       |      |
	| 1          | B	  |       |      |
	And messages in 'A' are
	| SequenceId | Node | Topic | Data |
	| 1          | A  |       |      |
	| 2          | A  |       |      |
	| 3          | A  |       |      |
	| 1          | B	  |       |      |
	And messages in 'B' are
	| SequenceId | Node | Topic | Data |
	| 1          | A  |       |      |
	| 1          | B	  |       |      |
	| 2          | B	  |       |      |
	When 'C' send UpdateRequest to 'B' 
	Then the response should be 
	| SequenceId | Node | Topic | Data |
	| 2          | B  |       |      |

Scenario: Cluster with 3 node and f(A) =2 f(B)=5 f(C) = 6
	Given Nodes of the cluster are
	| NodeName |
	|	A	   |  
	|	B	   |  
	|	C	   |
	And HeartBeat Frequency is 5
	And 'A' write 2 events/second
	And 'B' write 5 events/second
	And 'C' write 6 events/second
	And 'A' starts at 0
	And 'B' starts at 0
	And 'C' starts at 0
	Given Simulation ends at 100
	Then At 150 nodes are aligned
	And Messages are not duplicated
	And Sequence are respected

	Scenario: Cluster with 3 node and f(A) =1 f(B)=3 f(C) = 2
	Given Nodes of the cluster are
	| NodeName |
	|	A	   |  
	|	B	   |  
	|	C	   |
	And HeartBeat Frequency is 5
	And 'A' write 1 events/second
	And 'B' write 3 events/second
	And 'C' write 2 events/second
	And 'A' starts at 0
	And 'B' starts at 0
	And 'C' starts at 0
	Given Simulation ends at 100
	Then At 150 nodes are aligned
	And Messages are not duplicated
	And Sequence are respected

		Scenario: Cluster with 3 node and f(A) =5 f(B)=5 f(C) = 5
	Given Nodes of the cluster are
	| NodeName |
	|	A	   |  
	|	B	   |  
	|	C	   |
	And HeartBeat Frequency is 5
	And 'A' write 5 events/second
	And 'B' write 5 events/second
	And 'C' write 5 events/second
	And 'A' starts at 0
	And 'B' starts at 0
	And 'C' starts at 0
	Given Simulation ends at 100
	Then At 105 nodes are aligned
	And Messages are not duplicated
	And Sequence are respected

Scenario: Cluster with 3 node and f(A) =2 f(B)=2 f(C) = 2, A Starts at 20
	Given Nodes of the cluster are
	| NodeName |
	|	A	   |  
	|	B	   |  
	|	C	   |
	And HeartBeat Frequency is 5
	And 'A' write 2 events/second
	And 'B' write 2 events/second
	And 'C' write 2 events/second
	And 'A' starts at 20
	And 'B' starts at 0
	And 'C' starts at 0
	Given Simulation ends at 100
	Then At 105 nodes are aligned
	And Messages are not duplicated
	And Sequence are respected

	Scenario: Cluster with 3 node and f(A) =2 f(B)=2 f(C) = 2, A,B Starts at 20
	Given Nodes of the cluster are
	| NodeName |
	|	A	   |  
	|	B	   |  
	|	C	   |
	And HeartBeat Frequency is 5
	And 'A' write 2 events/second
	And 'B' write 2 events/second
	And 'C' write 2 events/second
	And 'A' starts at 20
	And 'B' starts at 20
	And 'C' starts at 0
	Given Simulation ends at 100
	Then At 105 nodes are aligned
	And Messages are not duplicated
	And Sequence are respected

	Scenario: Cluster with 3 node and f(A) =2 f(B)=2 f(C) = 2, C Starts at 20, B Stops at 20, B restart at 100
	Given Nodes of the cluster are
	| NodeName |
	|	A	   |  
	|	B	   |  
	|	C	   |
	And HeartBeat Frequency is 5
	And 'A' write 2 events/second
	And 'B' write 2 events/second
	And 'C' write 2 events/second
	And 'A' starts at 0
	And 'B' starts at 0
	And 'C' starts at 20
	And 'B' stops at 20
	And 'B' starts at 100
	Given Simulation ends at 100
	Then At 105 nodes are aligned
	And Messages are not duplicated
	And Sequence are respected

