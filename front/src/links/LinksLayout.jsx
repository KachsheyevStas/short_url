import { Routes, Route } from 'react-router-dom';

import { List, AddEdit } from '.';

export { LinksLayout };

function LinksLayout() {
    return (
        <div className="p-4">
            <div className="container">
                <Routes>
                    <Route index element={<List />} />
                    <Route path="add" element={<AddEdit />} />
                    <Route path="edit/:id" element={<AddEdit />} />
                </Routes>
            </div>
        </div>
    );
}
