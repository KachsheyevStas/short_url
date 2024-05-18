import { useEffect  } from 'react';
import { Link } from 'react-router-dom';
import { useSelector, useDispatch } from 'react-redux';

import { linkActions } from '_store';
export { List };

function List() {
    const links = useSelector(x => x.links.list);
    const dispatch = useDispatch();
    const baseUrl = `${process.env.REACT_APP_API_URL}`

    useEffect(() => {
        dispatch(linkActions.getAll());
    }, []);


    return (
        <div>
            <h1>Short Url</h1>
            <Link to="add" className="btn btn-sm btn-success mb-2">Add Url</Link>
            <table className="table table-striped">
                <thead>
                    <tr>
                        <th style={{ width: '30%' }}>Original Url</th>
                        <th style={{ width: '30%' }}>Created</th>
                        <th style={{ width: '30%' }}>Short URL</th>
                        <th style={{ width: '30%' }}>All Clicks</th>
                        <th style={{ width: '10%' }}></th>
                    </tr>
                </thead>
                <tbody>
                    {links?.value?.map(link =>
                        <tr key={link.id}>
                            <td>{link.originalUrl}</td>
                            <td>{link.createDate}</td>
                            <td>
                                <a  href={`${baseUrl}/${link.shortUrl}`} target="_blank">{`${baseUrl}/${link.shortUrl}`}</a>
                             </td>

                            <td>{link.countClick}</td>
                            
                            <td style={{ whiteSpace: 'nowrap' }}>
                                <Link to={`edit/${link.id}`} className="btn btn-sm btn-primary me-1">Edit</Link>
                                <button onClick={() => dispatch(linkActions.delete(link.id))} className="btn btn-sm btn-danger" style={{ width: '60px' }} disabled={link.isDeleting}>
                                    {link.isDeleting 
                                        ? <span className="spinner-border spinner-border-sm"></span>
                                        : <span>Delete</span>
                                    }
                                </button>
                            </td>
                        </tr>
                    )}
                    {links?.loading &&
                        <tr>
                            <td colSpan="4" className="text-center">
                                <span className="spinner-border spinner-border-lg align-center"></span>
                            </td>
                        </tr>
                    }
                </tbody>
            </table>
        </div>
    );
}
