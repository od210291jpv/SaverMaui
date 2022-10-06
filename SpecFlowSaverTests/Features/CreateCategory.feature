Feature: CreateNewCategory
!Endpoint for creation of new Category

@tag1
Scenario: CreateCategory	
	When I call endpoint for create the category with name "Test"
	Then I can find this category in results of GetAllCategory endpoint

Scenario: DeleteExistingCategory
	Given Category 'Test'
	When the category is available by GetCategories endpoint
	Then I can delete the category
	And the deleted category should not be available anymore in GetCategories results
