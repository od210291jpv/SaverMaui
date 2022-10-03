Feature: CreateNewCategory
!Endpoint for creation of new Category

@tag1
Scenario: CreateCategory	
	When I call endpoint for create the category with name "Test"
	Then I can find this category in results of GetAllCategory endpoint
