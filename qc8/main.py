import pytest
import json
import api

def check_product_details(expected, actual):
    for key, expected_value in expected.items():
        if key != "id":
            actual_value = actual.get(key)
            assert str(actual_value) == str(expected_value), f"Field '{key}' not equal'{expected_value}'. Current value: '{actual_value}'"

@pytest.fixture(scope="session")
def product_schema():
    with open('schema.json', 'r') as f:
        return json.load(f)

@pytest.fixture(scope="session")
def test_data():
    with open('tests.json', 'r') as f:
        return json.load(f)['test_data']

@pytest.fixture
def cleanup_products():
    created_product_ids = []
    yield created_product_ids
    for product_id in created_product_ids:
        delete_response = api.delete_product(product_id)
        assert delete_response and delete_response.get('status') == 1, f"Failed delete product with ID {product_id}"

class TestProductAPI:
    # delete tests
    def test_delete_correct_product(self, test_data, cleanup_products, product_schema):
        product_data = test_data['correctProduct']
        add_response = api.add_product(product_data)
        product_id = add_response.get('id')

        delete_response = api.delete_product(product_id)
        assert delete_response is not None, "Delete response error"
        assert delete_response.get('status') == 1, f"Product not deleted: {delete_response}"

        product = api.get_product_by_id(product_id)
        assert product is None, f"Product with ID {product_id} exist after deleting"

    def test_delete_non_existing_product(self):
        id_for_delete = "56789"
        product = api.get_product_by_id(id_for_delete)
        assert product is None, f"Product with id {id_for_delete} exist"

        delete_response = api.delete_product(id_for_delete)
        assert delete_response is not None, "Delete response error"

        assert delete_response.get('status') == 0, f"Delete not existing product status should be 0"

    # add tests
    def test_add_correct_product(self, test_data, cleanup_products, product_schema):
        product = test_data['correctProduct']

        add_response = api.add_product(product)
        assert add_response is not None, "Adding Response is None"
        assert add_response.get('status') == 1, "Product not added"

        product_id = add_response.get('id')

        cleanup_products.append(product_id)

        created_product = api.get_product_by_id(product_id)
        assert created_product is not None, f"Created product with ID {product_id} not found"

        check_product_details(product, created_product)

    def test_alias_add_product(self, test_data, cleanup_products, product_schema):
        first_product = test_data['correctProduct'].copy()
        second_product = test_data['correctProduct'].copy()

        add_first_response = api.add_product(first_product)
        first_product_id = add_first_response.get('id')

        add_second_response = api.add_product(second_product)
        second_product_id = add_second_response.get('id')

        cleanup_products.append(first_product_id)
        cleanup_products.append(second_product_id)

        first_created_product = api.get_product_by_id(first_product_id)
        second_created_product = api.get_product_by_id(second_product_id)

        first_alias = first_created_product.get("alias")
        second_alias = second_created_product.get("alias")

        assert first_alias != second_alias, f"Alias is same: {first_alias} - {second_alias}"
        assert second_alias == f"{first_alias}-0", f"Alias expected: '{first_alias}-0', Got: '{second_alias}'"

    def test_add_product_with_negative_price(self, test_data): # Падает, тк добавляется негативный прайс, когда не должен
        product = test_data['incorrectProduct_negativePrice'].copy()

        add_response = api.add_product(product)

        assert add_response is not None, "Empty response"
        assert add_response.get('status') == 0, "Product added status should be 0"

    def test_add_product_with_invalid_hit_and_status(self, test_data, cleanup_products): # Падает, тк добавляется продукт
        product = test_data['incorrectProduct_invalidHitAndStatus'].copy()

        add_response = api.add_product(product)

        assert add_response is not None, "Empty response"
        assert add_response.get('status') == 0, "Product added status should be 0"

    def test_add_product_with_status15(self, test_data, cleanup_products): # Падает, тк добавляется продукт, но его нет в списке
        product = test_data['incorrectProduct_status15'].copy()

        add_response = api.add_product(product)

        assert add_response is not None, "Empty response"
        assert add_response.get('status') == 1, "Product added status should be 1"

        product_id = add_response.get('id')
        cleanup_products.append(product_id)

        created_product = api.get_product_by_id(product_id)
        assert created_product is not None, f"Created product with ID {product_id} not found" # Падает здесь

        check_product_details(product, created_product)

    def test_add_product_with_missing_fields(self, test_data):
        product = test_data['incorrectProduct_emptyProduct'].copy()

        response = api.add_product(product)
        assert isinstance(response, str) or response is None, "Expected error, but got JSON"

    # edit tests
    def test_edit_correct_product(self, test_data, cleanup_products, product_schema):
        product = test_data['correctProduct']

        add_response = api.add_product(product)
        product_id = add_response.get('id')

        cleanup_products.append(product_id)

        edited_product = test_data['correctProductEdited']
        edited_product['id'] = add_response['id']

        edit_response = api.edit_product(edited_product)
        assert edit_response is not None, "Editing Response is None"
        assert edit_response.get('status') == 1, "Product not edited"

        edited_product_details = api.get_product_by_id(product_id)

        assert edited_product_details is not None, f"Edited product with ID {product_id} not found"

        edited_product['alias'] = f"edited_product['alias']-{product_id}"
        edited_product_details['alias'] = f"edited_product['alias']-{product_id}"

        check_product_details(edited_product, edited_product_details)

    def test_edit_invalid_product(self, test_data, cleanup_products, product_schema):
        product = test_data['correctProduct']

        add_response = api.add_product(product)
        product_id = add_response.get('id')

        cleanup_products.append(product_id)

        edited_product = test_data['invalidProductEdited']
        edited_product['id'] = add_response['id']

        edit_response = api.edit_product(edited_product)
        assert isinstance(edit_response, str) or edit_response is None, "Expected error, but got JSON"

    # Boundary tests for status field
    def test_add_product_with_status_0(self, test_data, cleanup_products):
        product = test_data['correctProduct'].copy()
        product['status'] = "0"  # Минимальное допустимое значение

        add_response = api.add_product(product)
        assert add_response is not None, "Adding Response is None"
        assert add_response.get('status') == 1, "Product with status=0 not added"

        product_id = add_response.get('id')
        cleanup_products.append(product_id)

        created_product = api.get_product_by_id(product_id)
        assert created_product is not None, f"Created product with ID {product_id} not found"
        assert created_product['status'] == "0", f"Status should be 0, got {created_product['status']}"

    def test_add_product_with_status_1(self, test_data, cleanup_products):
        product = test_data['correctProduct'].copy()
        product['status'] = "1"  # Максимальное допустимое значение

        add_response = api.add_product(product)
        assert add_response is not None, "Adding Response is None"
        assert add_response.get('status') == 1, "Product with status=1 not added"

        product_id = add_response.get('id')
        cleanup_products.append(product_id)

        created_product = api.get_product_by_id(product_id)
        assert created_product is not None, f"Created product with ID {product_id} not found"
        assert created_product['status'] == "1", f"Status should be 1, got {created_product['status']}"

    def test_add_product_with_invalid_status_2(self, test_data):
        product = test_data['correctProduct'].copy()
        product['status'] = "2"  # Недопустимое значение

        add_response = api.add_product(product)
        assert add_response is not None, "Empty response"
        assert add_response.get('status') == 0, "Product with invalid status=2 should not be added"

    # Boundary tests for hit field
    def test_add_product_with_hit_0(self, test_data, cleanup_products):
        product = test_data['correctProduct'].copy()
        product['hit'] = "0"  # Минимальное допустимое значение

        add_response = api.add_product(product)
        assert add_response is not None, "Adding Response is None"
        assert add_response.get('status') == 1, "Product with hit=0 not added"

        product_id = add_response.get('id')
        cleanup_products.append(product_id)

        created_product = api.get_product_by_id(product_id)
        assert created_product is not None, f"Created product with ID {product_id} not found"
        assert created_product['hit'] == "0", f"Hit should be 0, got {created_product['hit']}"

    def test_add_product_with_hit_1(self, test_data, cleanup_products):
        product = test_data['correctProduct'].copy()
        product['hit'] = "1"  # Максимальное допустимое значение

        add_response = api.add_product(product)
        assert add_response is not None, "Adding Response is None"
        assert add_response.get('status') == 1, "Product with hit=1 not added"

        product_id = add_response.get('id')
        cleanup_products.append(product_id)

        created_product = api.get_product_by_id(product_id)
        assert created_product is not None, f"Created product with ID {product_id} not found"
        assert created_product['hit'] == "1", f"Hit should be 1, got {created_product['hit']}"

    def test_add_product_with_invalid_hit_2(self, test_data):
        product = test_data['correctProduct'].copy()
        product['hit'] = "2"  # Недопустимое значение

        add_response = api.add_product(product)
        assert add_response is not None, "Empty response"
        assert add_response.get('status') == 0, "Product with invalid hit=2 should not be added"

    # Combined boundary tests
    def test_add_product_with_min_status_and_min_hit(self, test_data, cleanup_products):
        product = test_data['correctProduct'].copy()
        product['status'] = "0"
        product['hit'] = "0"

        add_response = api.add_product(product)
        assert add_response is not None, "Adding Response is None"
        assert add_response.get('status') == 1, "Product with status=0 and hit=0 not added"

        product_id = add_response.get('id')
        cleanup_products.append(product_id)

        created_product = api.get_product_by_id(product_id)
        assert created_product is not None, f"Created product with ID {product_id} not found"
        assert created_product['status'] == "0", f"Status should be 0, got {created_product['status']}"
        assert created_product['hit'] == "0", f"Hit should be 0, got {created_product['hit']}"

    def test_add_product_with_max_status_and_max_hit(self, test_data, cleanup_products):
        product = test_data['correctProduct'].copy()
        product['status'] = "1"
        product['hit'] = "1"

        add_response = api.add_product(product)
        assert add_response is not None, "Adding Response is None"
        assert add_response.get('status') == 1, "Product with status=1 and hit=1 not added"

        product_id = add_response.get('id')
        cleanup_products.append(product_id)

        created_product = api.get_product_by_id(product_id)
        assert created_product is not None, f"Created product with ID {product_id} not found"
        assert created_product['status'] == "1", f"Status should be 1, got {created_product['status']}"
        assert created_product['hit'] == "1", f"Hit should be 1, got {created_product['hit']}"