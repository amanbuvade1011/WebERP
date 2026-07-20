export interface CategoryNode {
  id: string;
  description: string;
  inactive: boolean;
  children: CategoryNode[];
}

export interface CreateCategoryRequest {
  description: string;
  parentId: string | null;
}

export interface Label {
  id: string;
  description: string;
}

export interface RangeNode {
  id: string;
  description: string | null;
  children: RangeNode[];
}

export interface Season {
  id: string;
  description: string;
  code: string;
}
